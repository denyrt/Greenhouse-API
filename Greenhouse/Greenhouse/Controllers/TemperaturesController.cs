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
    [Route("api/temperatures")]
    [ApiController]
    [Authorize]
    public class TemperaturesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IUserHelper _userHelper;
        private readonly IEmailService _emailService;

        public TemperaturesController(AppDbContext appDbContext,
            IUserHelper userHelper,
            IEmailService emailService)
        {
            _appDbContext = appDbContext;
            _userHelper = userHelper;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<ActionResult<TemperatureControllerModel>> Create(
            [FromBody] TemperatureControllerModel model)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var controller = new TemperatureController
            {
                Name = model.Name,
                ReportIntervalSeconds = model.ReportIntervalSeconds,
                UserId = currentUserId,

                CelsiusToStartAeration = model.CelsiusToStartAeration,
                CelsiusToFinishAeration = model.CelsiusToFinishAeration,
                CelsiusToStartHeating = model.CelsiusToStartHeating,
                CelsiusToFinishHeating = model.CelsiusToFinishHeating,

                BottomEdgeToAlert = model.BottomEdgeToAlert,
                UpperEdgeToAlert = model.UpperEdgeToAlert
            };

            try
            {
                await _appDbContext.AddAsync(controller);
                await _appDbContext.SaveChangesAsync();
                return Ok(controller.ToTemperatureModel());
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<TemperatureControllerModel[]>> GetAll([FromQuery] FilterModel filterModel)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var temperatures = _appDbContext.TemperatureControllers
                .Where(entry => entry.UserId == currentUserId);

            if (filterModel.AvailableForAdding == true)
            {
                temperatures = temperatures.Where(entry => entry.GreenhouseId == null);
            }
            
            var result = await temperatures.Select(entry => entry.ToTemperatureModel())
                .AsNoTracking()
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TemperatureControllerModel>> GetById([FromRoute] Guid id)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var temperature = await _appDbContext.TemperatureControllers
                .Where(entry => entry.UserId == currentUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (temperature == null)
            {
                return NotFound();
            }           

            return Ok(temperature.ToTemperatureModel());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var temperature = await _appDbContext.TemperatureControllers
                .Where(entry => entry.UserId == currentUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.Id == id);

            if (temperature == null)
            {
                return NotFound();
            }

            try
            {
                _appDbContext.TemperatureControllers.Remove(temperature);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult<TemperatureControllerModel>> Edit(
            [FromBody] TemperatureControllerModel editModel)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var temperature = await _appDbContext.TemperatureControllers
                .Where(entry => entry.UserId == currentUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.Id == editModel.Id);

            if (temperature == null)
            {
                return NotFound();
            }

            try
            {
                temperature.Name = editModel.Name;
                temperature.ReportIntervalSeconds = editModel.ReportIntervalSeconds;

                temperature.CelsiusToStartAeration = editModel.CelsiusToStartAeration;
                temperature.CelsiusToFinishAeration = editModel.CelsiusToFinishAeration;

                temperature.CelsiusToStartHeating = editModel.CelsiusToStartHeating;
                temperature.CelsiusToFinishHeating = editModel.CelsiusToFinishHeating;
                
                temperature.UpperEdgeToAlert = editModel.UpperEdgeToAlert;
                temperature.BottomEdgeToAlert = editModel.BottomEdgeToAlert;
                
                _appDbContext.TemperatureControllers.Update(temperature);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }

            return Ok(temperature.ToTemperatureModel());
        }

        [HttpPatch("put-to-greenhouse")]
        public async Task<IActionResult> PutToGreenhouse([FromBody] PutToGreenhouseModel putModel)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var controller = await _appDbContext.TemperatureControllers
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
        public async Task<ActionResult<TemperatureControllerModel>> CreateReport(
            [FromBody] CreateTemperatureReportModel createModel)
        {
            var currentUser = await _userHelper.GetCurrentUserAsync();
            var controller = await _appDbContext.TemperatureControllers.FindAsync(createModel.ControllerId);
            
            if (controller == null)
            {
                return NotFound();
            }

            var report = new TemperatureReport
            {
                ControllerId = createModel.ControllerId,
                Celsius = createModel.Celsius
            };

            try
            {
                await _appDbContext.TemperatureReports.AddAsync(report);
                await _appDbContext.SaveChangesAsync();

                if (createModel.Celsius >= controller.UpperEdgeToAlert)
                {
                    var sent = await _emailService.SendMessageAsync(currentUser.Email, 
                        "Temperature Alert",
                        $"Celsius temperature was {createModel.Celsius}.");
                }

                if (createModel.Celsius <= controller.BottomEdgeToAlert)
                {
                    var sent = await _emailService.SendMessageAsync(currentUser.Email,
                        "Temperature Alert",
                        $"Celsius temperature was {createModel.Celsius}.");
                }
            }
            catch (DbUpdateException exception)
            {
                return BadRequest();
            }

            return Ok(controller.ToTemperatureModel());
        }

        [HttpGet("reports")]
        public async Task<ActionResult<TemperatureReportViewModel[]>> GetReports(
            [FromQuery] ReportsFilter reportsFilter)
        {
            if (reportsFilter.StartDate > reportsFilter.EndDate)
            {
                return BadRequest("Invalid date range.");
            }

            var currentUserId = _userHelper.GetCurrentUserId();
            var results = await _appDbContext.Hothouses
                .Where(house => house.Id == reportsFilter.GreenhouseId)
                .Include(house => house.TemperatureControllers)
                .ThenInclude(controller => controller.TemperatureReports)
                .SelectMany(house => house.TemperatureControllers)
                .SelectMany(controller => controller.TemperatureReports)
                .Select(report => report.ToReportModel())
                .AsSingleQuery()
                .AsNoTracking()
                .ToListAsync();

            return Ok(results);
        }

        [HttpGet("reports/{id}")]
        public async Task<ActionResult<TemperatureReportViewModel>> GetResportById([FromRoute] Guid id)
        {
            var report = await _appDbContext.TemperatureReports.FindAsync(id);            
            if (report == null)
            {
                return NotFound();
            }

            return Ok(report.ToReportModel());
        }
    }
}
