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
    public class PropositoController : ControllerBase
    {
        private readonly PropositoService service;

        public PropositoController(IConfiguration configuration, ILogger<PropositoController> logger)
        {
            service = new PropositoService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetProposito")]
        public ActionResult<Proposito> GetProposito()
        {
            return service.BuscarProposito();
        }

        [HttpPost("PostSalvarProposito")]
        public ActionResult<Proposito> PostSalvarProposito([FromBody] Proposito proposito)
        {
            return service.SalvarProposito(proposito);
        }
    }
}