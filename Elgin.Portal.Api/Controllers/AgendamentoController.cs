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
    public class AgendamentoController : ControllerBase
    {
        private readonly AgendamentoService service;

        public AgendamentoController(IConfiguration configuration, ILogger<AgendamentoController> logger)
        {
            service = new AgendamentoService(new Config(configuration).ConnectionString, new Config(configuration).FileSettings);
        }

        [HttpGet("GetAgendamentos")]
        public ActionResult<List<Agendamento>> GetAgendamentos()
        {
            return service.ListarAgendamentos();
        }

        [HttpGet("GetAgendamentoPorId/{idAgendamento}")]
        public ActionResult<Agendamento> GetAgendamentoPorId(Guid idAgendamento)
        {
            return service.ProcuraAgendamentoPorId(idAgendamento);
        }

        [HttpGet("GetAgendamentoUsuarioPorId/{idAgendamentoUsuario}")]
        public ActionResult<AgendamentoUsuario> GetAgendamentoUsuarioPorId(Guid idAgendamentoUsuario)
        {
            return service.ProcuraAgendamentoUsuarioPorId(idAgendamentoUsuario);
        }

        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar(Agendamento agendamento)
        {
            return service.Salvar(agendamento);
        }

        [HttpGet("InativarAgendamento/{idAgendamento}")]
        public ActionResult<bool> InativarAgendamento(Guid idAgendamento)
        {
            return service.InativarAgendamento(idAgendamento);
        }
    }
}