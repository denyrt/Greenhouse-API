using Greenhouse.Data.Contexts;
using Greenhouse.Data.Models;
using Greenhouse.Models;
using Greenhouse.Services.Extensions;
using Greenhouse.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Greenhouse.Controllers
{
    [Route("api/greenhouses")]
    [ApiController]
    [Authorize]
    public class GreenhousesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IUserHelper _userHelper;
        private readonly ILogger _logger;

        public GreenhousesController(AppDbContext appDbContext,
            IUserHelper userHelper,
            ILogger logger)
        {
            _appDbContext = appDbContext;
            _userHelper = userHelper;
            _logger = logger;
        }        

        [HttpPost]
        public async Task<ActionResult<GreenhouseModel>> Create([FromBody] CreateGreenhouseModel greenhouseModel) 
        {
            var greenhouse = new Hothouse
            {
                UserId = _userHelper.GetCurrentUserId(),
                Name = greenhouseModel.Name
            };

            try
            {
                await _appDbContext.Hothouses.AddAsync(greenhouse);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(string.Format("[{0}]: {1}", DateTime.Now, exception.Message));
                return BadRequest();
            }

            return Ok(new GreenhouseModel
            {
                Id = greenhouse.Id,
                Name = greenhouse.Name,
                LightingControllers = Array.Empty<LightingControllerModel>(),
                TemperatureControllers = Array.Empty<TemperatureControllerModel>(),
                WetControllers = Array.Empty<WetControllerModel>()
            });
        }
        
        [HttpGet]
        public async Task<ActionResult<GreenhouseModel[]>> GetAll()
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var greenhouses = await _appDbContext.Hothouses
                .Where(greenhouse => greenhouse.UserId == currentUserId)
                .AsSingleQuery()
                .Select(greenhouse => new GreenhouseModel
                {
                    Id = greenhouse.Id,
                    Name = greenhouse.Name,
                    TemperatureControllers = greenhouse.TemperatureControllers
                                            .Select(entry => entry.ToTemperatureModel())
                                            .ToList(),
                    LightingControllers = greenhouse.LightControllers
                                          .Select(entry => entry.ToLightModel())
                                          .ToList(),
                    WetControllers = greenhouse.WetControllers
                                          .Select(entry => entry.ToWetModel())
                                          .ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            return Ok(greenhouses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GreenhouseModel>> GetById([FromRoute] Guid id) 
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var greenhouse = await _appDbContext.Hothouses
                .Where(greenhouse => greenhouse.UserId == currentUserId)
                .AsSingleQuery()
                .Select(greenhouse => new GreenhouseModel
                {
                    Id = greenhouse.Id,
                    Name = greenhouse.Name,
                    TemperatureControllers = greenhouse.TemperatureControllers
                                            .Select(entry => entry.ToTemperatureModel())
                                            .ToList(),
                    LightingControllers = greenhouse.LightControllers
                                          .Select(entry => entry.ToLightModel())
                                          .ToList(),
                    WetControllers = greenhouse.WetControllers
                                          .Select(entry => entry.ToWetModel())
                                          .ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(greenhouse => greenhouse.Id == id);

            return Ok(greenhouse);
        }        

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id) 
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var greenhouse = await _appDbContext.Hothouses
                .Where(entry => entry.UserId == currentUserId)
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.Id == id);

            if (greenhouse == null)
            {
                return NotFound();
            }

            try
            {
                _appDbContext.Hothouses.Remove(greenhouse);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(string.Format("[{0}]: {1}", DateTime.Now, exception.Message));
                return BadRequest();
            }

            return NoContent();
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<GreenhouseModel>> EditName([FromRoute] Guid id,
            [FromBody] UpdateGreenhouseModel greenhouseModel)
        {
            var currentUserId = _userHelper.GetCurrentUserId();
            var greenhouse = await _appDbContext.Hothouses
                .Where(entry => entry.UserId == currentUserId)
                .Include(entry => entry.LightControllers)
                .Include(entry => entry.TemperatureControllers)
                .Include(entry => entry.WetControllers)
                .AsSingleQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(entry => entry.Id == id);

            if (greenhouse == null)
            {
                return NotFound();
            }

            try
            {
                greenhouse.Name = greenhouseModel.Name;
                _appDbContext.Update(greenhouse);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                _logger.LogError(string.Format("[{0}]: {1}", DateTime.Now, exception.Message));
                return BadRequest();
            }

            var model = new GreenhouseModel
            {
                Id = greenhouse.Id,
                Name = greenhouse.Name,
                TemperatureControllers = greenhouse.TemperatureControllers
                                          .Select(entry => entry.ToTemperatureModel())
                                          .ToList(),
                LightingControllers = greenhouse.LightControllers
                                          .Select(entry => entry.ToLightModel())
                                          .ToList(),
                WetControllers = greenhouse.WetControllers
                                          .Select(entry => entry.ToWetModel())
                                          .ToList()
            };

            return Ok(model);
        }
    }
}
