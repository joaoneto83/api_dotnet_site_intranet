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
    public class EventoController : ControllerBase
    {
        private readonly ILogger<EventoController> _logger;
        private readonly EventoService service;

        public EventoController(IConfiguration configuration, ILogger<EventoController> logger)
        {
            _logger = logger;
            service = new EventoService(new Config(configuration).ConnectionString);
        }

        [HttpGet]
        public ActionResult<List<Evento>> Get()
        {
            return service.Listar();
        }

        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar([FromBody] Evento model)
        {
            return service.Salvar(model);
        }

        [HttpGet("ListarEventosParaCalendario/{ano}")]
        public ActionResult<List<Feriado>> ListarEventosParaCalendario(int ano)
        {
            return service.ListarEventosParaCalendario(ano);
        }
    }
}