using System;
using System.Collections.Generic;
using Elgin.Portal.Api.Controllers.Shared;
using Elgin.Portal.Services.Implementation;
using Elgin.Portal.Services.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RHController : ControllerBase
    {
        private readonly RHService _service;

        public RHController(IConfiguration configuration, ILogger<RHController> logger)
        {
            _service = new RHService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetRH")]
        public ActionResult<RH> GetRH()
        {
            return _service.GetRH();
        }
    }
}