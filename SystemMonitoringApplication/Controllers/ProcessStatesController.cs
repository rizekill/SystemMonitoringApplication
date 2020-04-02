using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SM.Domain.Interfaces;
using SM.Model;

namespace SystemMonitoringApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessStatesController : ControllerBase
    {
        private readonly IMonitorService _monitorService;

        public ProcessStatesController(IMonitorService monitorService)
        {
            _monitorService = monitorService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProcessState>> Get()
            => Ok(_monitorService.GetProcessStates());
    }
}
