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
    public class AssistenciaController : ControllerBase
    {
        private readonly AssistenciaService service;
        private readonly EmailService emailService;
        private readonly DominioService dominioService;

        public AssistenciaController(IConfiguration configuration, ILogger<AssistenciaController> logger)
        {
            service = new AssistenciaService(new Config(configuration).ConnectionString);
            dominioService = new DominioService(new Config(configuration).ConnectionString);
            emailService = new EmailService(new Config(configuration).ConnectionString, new Config(configuration).EmailSettings, new Config(configuration).FileSettings);
        }

        [HttpGet("GetAssistencias/{uf}/{idCidade}")]
        public ActionResult<List<Assistencia>> GetAssistencias(string uf, int idCidade)
        {
            return service.Listar(uf, idCidade);
        }
        [HttpGet("GetCidade/{uf}")]
        public ActionResult<List<Cidade>> GetCidade(string uf)
        {
            return service.BuscarCidade(uf);
        }

        [HttpGet("GetCidades/{estado}")]
        public ActionResult<List<Cidade>> GetCidades(string estado)
        {
            return service.ListaCidades(estado);
        }

        [HttpGet("Listar")]
        public ActionResult<List<Assistencia>> Listar()
        {
            return service.Listar();
        }

        [HttpGet("{id}")]
        public ActionResult<Assistencia> Get(Guid id)
        {
            return service.GetAssistencia(id);
        }

        [HttpPost]
        public ActionResult<bool> Salvar(Assistencia model)
        {
            return service.Salvar(model);
        }

        [HttpPost("SejaAssistenciaTecnica")]
        public async void SejaAssistenciaTecnica([FromBody] FormularioAssistenciaTecnica formulario)
        {
            emailService.EnviarSejaAssistencia(formulario);
        }

        [HttpGet("GetSolarIntegradores/{uf}/{idCidade?}")]
        public ActionResult<List<SolarIntegrador>> GetSolarIntegradores(string uf, int? idCidade)
        {
            return dominioService.getSolarIntegradores(uf, idCidade);
        }

        [HttpGet("GetAssistenciasLinha/{idLinha}")]
        public ActionResult<List<Assistencia>> GetAssistenciasLinha(Guid idLinha)
        {
            return service.ListarBuscar(idLinha);
        }

        [HttpGet("GetAssistenciasLinhaCidade/{idLinha}/{uf}/{idCidade}")]
        public ActionResult<List<Assistencia>> GetAssistenciasLinhaCidade(Guid idLinha, string uf, int idCidade)
        {
            return service.ListarBuscarCidade(idLinha, uf, idCidade);
        }
    }
}