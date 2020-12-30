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
    public class EventoRefrigeracaoController : ControllerBase
    {
        private readonly EventoRefrigeracaoService service;

        public EventoRefrigeracaoController(IConfiguration configuration, ILogger<EventoRefrigeracaoController> logger)
        {
            service = new EventoRefrigeracaoService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetEventos")]
        public ActionResult<List<EventoRefrigeracao>> GetEventos()
        {
            return service.Listar();
        }

        [HttpGet("GetEventoRefrigeracaoPorId/{idEventoRefrigeracao}")]
        public ActionResult<EventoRefrigeracao> GetEventoRefrigeracaoPorId(Guid idEventoRefrigeracao)
        {
            return service.ProcuraEventoRefrigeracaoPorId(idEventoRefrigeracao);
        }

        [Authorize]
        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar([FromBody] EventoRefrigeracao model)
        {
            return service.Salvar(model);
        }

        [Authorize]
        [HttpGet("DeleteEventoRefrigeracao/{idEventoRefrigeracao}")]
        public ActionResult<bool> DeleteEventoRefrigeracao(Guid idEventoRefrigeracao)
        {
            return service.Remover(idEventoRefrigeracao);
        }

        [HttpGet("FiltrarEventosRefrigeracao")]
        public ActionResult<List<EventoRefrigeracao>> FiltrarEventosRefrigeracao(int? idTipoEventoRefrigeracao = null, int? mes = null, bool apenasTreinamentos = false, string cultura = "pt-Br")
        {
            return service.Listar(idTipoEventoRefrigeracao, mes, cultura, apenasTreinamentos);
        }
    }
}