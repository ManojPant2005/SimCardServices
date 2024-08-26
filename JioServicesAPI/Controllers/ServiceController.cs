using JioServicesAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JioServicesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly NotificationService _notificationService;
        private readonly IHostApplicationLifetime _lifetime;

        public ServiceController(NotificationService notificationService, IHostApplicationLifetime lifetime)
        {
            _notificationService = notificationService;
            _lifetime = lifetime;
        }

        [HttpPost("start")]
        public IActionResult StartService()
        {
            if (!_notificationService.IsRunning)
            {
                _notificationService.StartAsync(_lifetime.ApplicationStopping);
                return Ok("Service started.");
            }

            return BadRequest("Service is already running.");
        }

        [HttpPost("stop")]
        public IActionResult StopService()
        {
            if (_notificationService.IsRunning)
            {
                _notificationService.StopAsync(_lifetime.ApplicationStopping);
                return Ok("Service stopped.");
            }

            return BadRequest("Service is not running.");
        }

        [HttpGet("status")]
        public IActionResult GetServiceStatus()
        {
            var status = _notificationService.IsRunning ? "Service is running." : "Service is stopped.";
            return Ok(status);
        }
    }
}