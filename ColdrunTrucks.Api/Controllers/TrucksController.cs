using ColdrunTrucks.Api.Models;
using ColdrunTrucks.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;

namespace ColdrunTrucks.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TrucksController : ControllerBase
    {
        private readonly ITruckRepository _truckRepository;
        private readonly ILogger<TrucksController> _logger;

        public TrucksController(ITruckRepository truckService, ILogger<TrucksController> logger)
        {
            _truckRepository = truckService;
            _logger = logger;
        }

        /// <param name="sortBy">Can be sorted by: 'code', 'name', 'description', 'status'</param>
        [HttpGet("Get")]
        public async Task<IEnumerable<Truck>> Get([FromQuery] string? searchTerm, [FromQuery] string? sortBy)
        {
            var trucks = await _truckRepository.GetTrucksAsync(searchTerm, sortBy);
            return trucks;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(Truck truck)
        {
            try
            {
                await _truckRepository.AddAsync(truck);
            }
            catch (DbUpdateException dbException)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError("Unable to add entry {truckCode} to database. Inner exception: {exception}", truck.Code, dbException.Message);
                return StatusCode(400, "Unable to add entry to database. Most probable cause: duplicate truck code.");
            }
            return Ok();
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(Truck truck)
        {
            try
            {
                await _truckRepository.UpdateAsync(truck);
            }
            catch (InvalidOperationException operationException)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError("Unable to update entry {truckCode}. Inner exception: {exception}", truck.Code, operationException.Message);
                return StatusCode(400, string.Format("Unable to find entry with truck code {0}", truck.Code));
            }
            catch (InvalidTruckStatusException invalidStatusException) 
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError("Unable to update entry {truckCode}. Inner exception: {exception}", truck.Code, invalidStatusException.Message);
                return StatusCode(400, string.Format("Wrong status for truck code {0}", truck.Code));
            }
            return Ok();
        }

        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdateTruckStatus(string truckCode, TruckStatus status)
        {
            try
            {
                await _truckRepository.UpdateStatusAsync(truckCode, status);
            }
            catch (InvalidOperationException operationException)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError("Unable to update entry {truckCode}. Inner exception: {exception}", truckCode, operationException.Message);
                return StatusCode(400, string.Format("Unable to find entry with truck code {0}", truckCode));
            }
            catch (InvalidTruckStatusException invalidStatusException)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError("Unable to update entry {truckCode}. Inner exception: {exception}", truckCode, invalidStatusException.Message);
                return StatusCode(400, string.Format("wrong status for truck code {0}", truckCode));
            }
            return Ok();
        }

        [HttpPut("SetNextStatus")]
        public async Task<IActionResult> SetNextStatus(string truckCode)
        {
            try
            {
                await _truckRepository.SetNextStatusAsync(truckCode);
            }
            catch (InvalidOperationException operationException)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError("Unable to find entry {truckCode}. Inner exception: {exception}", truckCode, operationException.Message);
                return StatusCode(400, string.Format("Unable to find entry with truck code {0}", truckCode));
            }
            return Ok();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(string truckCode)
        {
            try
            {
                await _truckRepository.DeleteTruckAsync(truckCode);
            }
            catch (InvalidOperationException operationException)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                    _logger.LogError("Unable to find entry {truckCode}. Inner exception: {exception}", truckCode, operationException.Message);
                return StatusCode(400, string.Format("Unable to find entry with truck code {0}", truckCode));
            }
            return Ok();
        }
    }
}
