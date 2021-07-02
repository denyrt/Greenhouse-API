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
    [Route("api/wets")]
    [ApiController]
    [Authorize]
    public class WetsController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly AppDbContext _appDbContext;

        public WetsController(IUserHelper userHelper,
            AppDbContext appDbContext)
        {
            _userHelper = userHelper;
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<ActionResult<WetControllerModel>> Create(
            [FromBody] WetControllerModel model)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var wet = new WetController
            {
                UserId = currentUserId,
                Name = model.Name,
                ReportIntervalSeconds = model.ReportIntervalSeconds,

                BottomWetEdge = model.BottomWetEdge,
                UpperWetEdge = model.UpperWetEdge
            };

            try
            {
                await _appDbContext.WetControllers.AddAsync(wet);
                await _appDbContext.SaveChangesAsync();
                return Ok(wet.ToWetModel());
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<WetControllerModel[]>> GetAll([FromQuery] FilterModel filterModel)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var wets = _appDbContext.WetControllers
                .Where(entry => entry.UserId == currentUserId);

            if (filterModel.AvailableForAdding == true)
            {
                wets = wets.Where(entry => entry.GreenhouseId == null);
            }

            var result = await wets.Select(entry => entry.ToWetModel())
                                .AsNoTracking()
                                .ToListAsync();
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var wet = await _appDbContext.WetControllers
                .Where(entry => entry.UserId == currentUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.Id == id);

            if (wet == null)
            {
                return NotFound();
            }

            return Ok(wet.ToWetModel());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var wet = await _appDbContext.WetControllers
                .Where(entry => entry.UserId == currentUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.Id == id);

            if (wet == null)
            {
                return NotFound();
            }

            try
            {
                _appDbContext.WetControllers.Remove(wet);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult<WetControllerModel>> Edit(
            [FromBody] WetControllerModel editModel)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var wet = await _appDbContext.WetControllers
                .Where(entry => entry.UserId == currentUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.Id == editModel.Id);

            if (wet == null)
            {
                return NotFound();
            }

            try
            {
                wet.Name = editModel.Name;
                wet.ReportIntervalSeconds = editModel.ReportIntervalSeconds;
                wet.UpperWetEdge = editModel.UpperWetEdge;
                wet.BottomWetEdge = editModel.BottomWetEdge;                

                _appDbContext.WetControllers.Update(wet);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }

            return Ok(wet.ToWetModel());
        }

        [HttpPatch("put-to-greenhouse")]
        public async Task<IActionResult> PutToGreenhouse([FromBody] PutToGreenhouseModel putModel)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var controller = await _appDbContext.WetControllers
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
        public async Task<ActionResult<WetControllerModel>> CreateReport(
            [FromBody] CreateWetReportModel createModel)
        {
            var controller = await _appDbContext.WetControllers.FindAsync(createModel.ControllerId);

            if (controller == null)
            {
                return NotFound();
            }

            var report = new WetReport
            {
                ControllerId = createModel.ControllerId,
                WetPercent = createModel.WetPercent
            };

            try
            {
                await _appDbContext.WetReports.AddAsync(report);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }

            return Ok(controller.ToWetModel());
        }

        [HttpGet("reports")]
        public async Task<ActionResult<WetReportViewModel[]>> GetReports(
            [FromQuery] ReportsFilter reportsFilter)
        {
            if (reportsFilter.StartDate > reportsFilter.EndDate)
            {
                return BadRequest("Invalid date range.");
            }

            var currentUserId = _userHelper.GetCurrentUserId();
            var results = await _appDbContext.Hothouses
                .Where(house => house.Id == reportsFilter.GreenhouseId)
                .Include(house => house.WetControllers)
                .ThenInclude(controller => controller.WetReports)
                .SelectMany(house => house.WetControllers)
                .SelectMany(controller => controller.WetReports)
                .Select(report => report.ToReportModel())
                .AsSingleQuery()
                .AsNoTracking()
                .ToListAsync();

            return Ok(results);
        }

        [HttpGet("reports/{id}")]
        public async Task<ActionResult<WetReportViewModel>> GetResportById([FromRoute] Guid id)
        {
            var report = await _appDbContext.WetReports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            return Ok(report.ToReportModel());
        }
    }
}
