using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elgin.Portal.Api.Controllers.Shared;
using Elgin.Portal.Services.Implementation;
using Elgin.Portal.Services.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElginNewsController : ControllerBase
    {
        private readonly ElginNewsService service;

        public ElginNewsController(IConfiguration configuration, ILogger<ElginNewsController> logger)
        {
            service = new ElginNewsService(new Config(configuration).ConnectionString);
        }

        [HttpGet]
        public ActionResult<List<ElginNews>> Get()
        {
            return service.Listar();
        }

        [HttpPost("Salvar")]
        [Authorize]
        public ActionResult<bool> Salvar(ElginNews model)
        {
            return service.Salvar(model);
        }
    }
}