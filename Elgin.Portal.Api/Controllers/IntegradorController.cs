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
    public class IntegradorController : ControllerBase
    {
        private readonly IntegradorService service;

        public IntegradorController(IConfiguration configuration, ILogger<IntegradorController> logger)
        {
            service = new IntegradorService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetIntegradores")]
        public ActionResult<List<Integrador>> GetIntegradores()
        {
            return service.Listar();
        }

        [HttpGet("GetIntegradorPorId/{idIntegrador}")]
        public ActionResult<Integrador> GetIntegradorPorId(Guid idIntegrador)
        {
            return service.ProcuraIntegradorPorId(idIntegrador);
        }

        [Authorize]
        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar([FromBody] Integrador model)
        {
            return service.Salvar(model);
        }

        [HttpGet("FiltrarIntegradores")]
        public ActionResult<List<Integrador>> FiltrarIntegradores(int? idPais = null, string uf = null, string segmento = null)
        {
            return service.FiltrarIntegradores(idPais, uf, segmento);
        }
    }
}