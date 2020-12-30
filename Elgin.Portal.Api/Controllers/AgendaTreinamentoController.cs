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
    public class AgendaTreinamentoController : ControllerBase
    {
        private readonly AgendaTreinamentoService service;

        public AgendaTreinamentoController(IConfiguration configuration, ILogger<AgendaTreinamentoController> logger)
        {
            service = new AgendaTreinamentoService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetAgendaTreinamentos")]
        public ActionResult<List<AgendaTreinamento>> GetAgendaTreinamentos()
        {
            return service.Listar();
        }
    }
}