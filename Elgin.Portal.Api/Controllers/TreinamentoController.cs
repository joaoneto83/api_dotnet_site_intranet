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
    [Authorize]
    public class TreinamentoController : ControllerBase
    {
        private readonly TreinamentoService service;

        public TreinamentoController(IConfiguration configuration, ILogger<TreinamentoController> logger)
        {
            service = new TreinamentoService(new Config(configuration).ConnectionString, new Config(configuration).FileSettings);
        }

        [HttpGet("GetDocumentos")]
        public ActionResult<List<Linha>> GetDocumentos()
        {
            return service.getDocumentos();
        }

        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar([FromBody] DocumentoTreinamento model)
        {
            return service.salvar(model);
        }

    }
}