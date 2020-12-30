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
    public class DominioController : ControllerBase
    {
        private readonly DominioService service;

        public DominioController(IConfiguration configuration, ILogger<DominioController> logger)
        {
            service = new DominioService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetPaises")]
        public ActionResult<List<Pais>> GetPaises()
        {
            return service.ListaPaises();
        }

        [HttpGet("GetEstados")]
        public ActionResult<List<Estado>> GetEstados()
        {
            return service.ListarEstados();
        }

        [HttpGet("GetEstadosPorPais/{idPais}")]
        public ActionResult<List<Estado>> GetEstadosPorPais(int idPais)
        {
            return service.ListarEstados(idPais);
        }

        [HttpGet("GetCidades/{estado}")]
        public ActionResult<List<Cidade>> GetCidades(string estado)
        {
            return service.ListaCidades(estado);
        }

        [HttpGet("GetSegmentos")]
        public ActionResult<List<Segmento>> GetSegmentos()
        {
            return service.ListaSegmentos();
        }

        [HttpGet("GetTiposEventoRefrigeracao")]
        public ActionResult<List<TipoEventoRefrigeracao>> GetTiposEventoRefrigeracao()
        {
            return service.ListarTiposEventoRefrigeracao();
        }
    }
}