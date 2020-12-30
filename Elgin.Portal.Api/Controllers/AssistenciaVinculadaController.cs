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
    public class AssistenciaVinculadaController : ControllerBase
    {
        private readonly AssistenciaVinculadaService service;
        

        public AssistenciaVinculadaController(IConfiguration configuration, ILogger<AssistenciaVinculadaController> logger)
        {
            service = new AssistenciaVinculadaService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetAssistenciasVinculadas")]
        public ActionResult<List<AssistenciaVinculada>> GetAssistenciasVinculadas()
        {
            return service.Listar();
        }

        [HttpGet("GetAssistenciaPorId/{idAssistencia}")]
        public ActionResult<AssistenciaVinculada> GetAssistenciaPorId(Guid idAssistencia)
        {
            return service.GetAssistenciaPorId(idAssistencia);
        }
        
        [HttpGet("PostBuscaAssistenciasPorSubLinha/{idSubLinha}")]
        public ActionResult<List<AssistenciaVinculada>> PostBuscaAssistenciasPorSubLinha(Guid idSubLinha)
        {
            return service.BuscarAssistencias(idSubLinha);
        }

        [HttpPost("VincularSubLinhas")]
        public ActionResult<bool> VincularSubLinhas([FromForm] Guid idAssistencia, [FromForm] string documento, [FromForm] string idsSubLinhas)
        {
            return service.VincularSubLinhas(idAssistencia, documento, idsSubLinhas);
        }
    }
}