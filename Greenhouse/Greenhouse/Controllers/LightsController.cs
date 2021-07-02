using Greenhouse.Data.Contexts;
using Greenhouse.Data.Models;
using Greenhouse.Models;
using Greenhouse.Models.Reports;
using Greenhouse.Services.Extensions;
using Greenhouse.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Greenhouse.Controllers
{
    [Route("api/lights")]
    [ApiController]
    [Authorize]
    public class LightsController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly AppDbContext _appDbContext;

        public LightsController(IUserHelper userHelper,
            AppDbContext appDbContext)
        {
            _userHelper = userHelper;
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<ActionResult<LightingControllerModel>> Create(
            [FromBody] LightingControllerModel model)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var lighting = new LightController
            {
                UserId = currentUserId,
                Name = model.Name,
                ReportIntervalSeconds = model.ReportIntervalSeconds,

                LightingTimeSeconds = model.LightingSeconds,
                PermissibleLightingValue = model.PermissibleLightValue                
            };

            try
            {
                await _appDbContext.LightControllers.AddAsync(lighting);
                await _appDbContext.SaveChangesAsync();
                return Ok(lighting.ToLightModel());
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<LightingControllerModel[]>> GetAll([FromQuery] FilterModel filterModel)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var lights = _appDbContext.LightControllers
                .Where(entry => entry.UserId == currentUserId);

            if (filterModel.AvailableForAdding == true)
            {
                lights = lights.Where(entry => entry.GreenhouseId == null);
            }

            var result = await lights.Select(entry => entry.ToLightModel())
                .AsNoTracking()
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LightingControllerModel>> GetById([FromRoute] Guid id)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var lights = await _appDbContext.LightControllers
                .Where(entry => entry.UserId == currentUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.Id == id);

            if (lights == null)
            {
                return NotFound();
            }

            return Ok(lights.ToLightModel());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var light = await _appDbContext.LightControllers
                .Where(entry => entry.UserId == currentUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.Id == id);

            if (light == null)
            {
                return NotFound();
            }

            try
            {
                _appDbContext.LightControllers.Remove(light);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult<LightingControllerModel>> Edit(
            [FromBody] LightingControllerModel editModel)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var lighting = await _appDbContext.LightControllers
                .Where(entry => entry.UserId == currentUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.Id == editModel.Id);

            if (lighting == null)
            {
                return NotFound();
            }

            try
            {
                lighting.Name = editModel.Name;
                lighting.ReportIntervalSeconds = editModel.ReportIntervalSeconds;
                lighting.LightingTimeSeconds = editModel.LightingSeconds;
                lighting.PermissibleLightingValue = editModel.PermissibleLightValue;
                
                _appDbContext.LightControllers.Update(lighting);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }

            return Ok(lighting.ToLightModel());
        }

        [HttpPatch("put-to-greenhouse")]
        public async Task<IActionResult> PutToGreenhouse([FromBody] PutToGreenhouseModel putModel)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var controller = await _appDbContext.LightControllers
                .Where(entry => entry.UserId == currentUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.Id == putModel.ControllerId);

            if (putModel.GreenhouseId.HasValue)
            {
                var greenhouseExists = await _appDbContext.Hothouses
                    .Where(entry => entry.UserId == currentUserId)
                    .AsNoTracking()
                    .AnyAsync(entry => entry.Id == putModel.GreenhouseId);

                if (!greenhouseExists) return NotFound("Greenhouse not found.");
                controller.GreenhouseId = putModel.GreenhouseId;
            }
            else
            {
                controller.GreenhouseId = null;
            }

            try
            {
                _appDbContext.Update(controller);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpPost("reports")]
        public async Task<ActionResult<LightModel>> CreateReport(
            [FromBody] CreateLightReportModel createModel)
        {
            var controller = await _appDbContext.LightControllers.FindAsync(createModel.ControllerId);

            if (controller == null)
            {
                return NotFound();
            }

            var report = new LightReport
            {
                ControllerId = createModel.ControllerId,
                Lumens = createModel.Lumens
            };

            try
            {
                await _appDbContext.LightReports.AddAsync(report);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }

            var model = new LightModel
            {
                Seconds = 0,
                ReportIntervalSeconds = controller.ReportIntervalSeconds
            };        
            
            if (report.Lumens < controller.PermissibleLightingValue)
            {
                model.Seconds = controller.LightingTimeSeconds;
            };

            return Ok(model);
        }

        [HttpGet("reports")]
        public async Task<ActionResult<LightReportViewModel[]>> GetReports(
            [FromQuery] ReportsFilter reportsFilter)
        {
            if (reportsFilter.StartDate > reportsFilter.EndDate)
            {
                return BadRequest("Invalid date range.");
            }

            var currentUserId = _userHelper.GetCurrentUserId();
            var results = await _appDbContext.Hothouses
                .Where(house => house.Id == reportsFilter.GreenhouseId)
                .Include(house => house.LightControllers)
                .ThenInclude(controller => controller.LightReports)
                .SelectMany(house => house.LightControllers)
                .SelectMany(controller => controller.LightReports)
                .Select(report => report.ToReportModel())
                .AsSingleQuery()
                .AsNoTracking()
                .ToListAsync();

            return Ok(results);
        }

        [HttpGet("reports/{id}")]
        public async Task<ActionResult<TemperatureReportViewModel>> GetResportById([FromRoute] Guid id)
        {
            var report = await _appDbContext.LightReports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            return Ok(report.ToReportModel());
        }
    }
}
