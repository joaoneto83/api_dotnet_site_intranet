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
    public class SolarIntegradorController : ControllerBase

    {
        private readonly SolarIntegradorService service;
        private readonly DominioService dominioService;


        public SolarIntegradorController(IConfiguration configuration, ILogger<SolarIntegradorController> logger)
        {
            service = new SolarIntegradorService(new Config(configuration).ConnectionString);
            dominioService = new DominioService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetIntegradores")]
        public ActionResult<List<SolarIntegrador>> GetIntegradores()
        {
            return service.Listar();
        }

        [HttpGet("GetIntegradorPorId/{idIntegrador}")]
        public ActionResult<SolarIntegrador> GetIntegradorPorId(Guid idIntegrador)
        {
            return service.ProcuraIntegradorPorId(idIntegrador);
        }

        [Authorize]
        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar([FromBody] SolarIntegrador model)
        {
            return service.Salvar(model);
        }



    }
}