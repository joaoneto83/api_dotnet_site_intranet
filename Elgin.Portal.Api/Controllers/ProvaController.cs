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
    public class ProvaController : ControllerBase
    {
        private readonly ProvaService service;
        

        public ProvaController(IConfiguration configuration, ILogger<ProvaController> logger)
        {
            service = new ProvaService(new Config(configuration).ConnectionString, new Config(configuration).FileSettings);
        }

        [HttpGet("GetProvas/")]
        public ActionResult<List<Prova>> GetProvas()
        {
            return service.ListarProvas();
        }

        [HttpGet("GetProvasPorUsuario/{idUsuario}")]
        public ActionResult<TreinamentoUsuario> GetProvasPorUsuario(Guid idUsuario)
        {
            return service.RetornaTreinamentoUsuario(idUsuario);
        }

        [HttpGet("GetProvaPorId/{idProva}")]
        public ActionResult<Prova> GetProvaPorId(Guid idProva)
        {
            return service.ProcuraProvaPorId(idProva);
        }

        [HttpGet("AddProva/{idUsuario}")]
        public ActionResult<Prova> AddProva(Guid idUsuario)
        {
            return service.AddProva(idUsuario);
        }

        [HttpPost("AddQuestao")]
        public ActionResult<Questao> AddQuestao(Questao questao)
        {
            return service.AddQuestao(questao);
        }

        [HttpPost("AddAlternativa")]
        public ActionResult<Alternativa> AddAlternativa(Alternativa alternativa)
        {
            return service.AddAlternativa(alternativa);
        }

        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar([FromBody] Prova model)
        {
            return service.Salvar(model);
        }

        [HttpGet("InativarProva/{idProva}")]
        public ActionResult<bool> InativarProva(Guid idProva)
        {
            return service.InativarProva(idProva);
        }

        [HttpGet("AbrirProva/{idAgendamento}/{idUsuario}")]
        public ActionResult<Prova> AbrirProva(Guid idAgendamento, Guid idUsuario)
        {
            return service.AbrirProva(idAgendamento, idUsuario);
        }

        [HttpPost("FinalizarProva")]
        public ActionResult<Prova> FinalizarProva(Prova prova)
        {
            return service.FinalizarProva(prova);
        }

        [HttpGet("GetAproveitamento/{idAgendamento}")]
        public ActionResult<ProvaUsuario> GetAproveitamento(Guid idAgendamento)
        {
            return service.RetornaAproveitamento(idAgendamento);
        }

        [HttpPost("GetResultado")]
        public ActionResult<Resultado> GetResultado([FromBody] Resultado resultado)
        {
            return service.RetornaResultado(resultado);
        }

        [HttpPost("SalvarParcial")]
        public void SalvarParcial(Questao model)
        {
            service.SalvarParcial(model);
        }

    }
}