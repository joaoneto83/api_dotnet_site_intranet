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
using Newtonsoft.Json;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly EmailService service;
        private readonly ProdutoService produtoService;

        public EmailController(IConfiguration configuration, ILogger<EmailController> logger)
        {
            _logger = logger;
            service = new EmailService(new Config(configuration).ConnectionString, new Config(configuration).EmailSettings,
                                         new Config(configuration).FileSettings);

            produtoService = new ProdutoService(new Config(configuration).ConnectionString,
                                         new Config(configuration).FileSettings);
        }

        [HttpPost("EnviarEmail")]
        public void EnviarEmail(Email model)
        {
            service.EnviarEmail(model);
        }

        [HttpPost("NovoUsuario")]
        public void NovoUsuario([FromForm]string model)
        {
           var usuario = JsonConvert.DeserializeObject<Usuario>(model);

           service.NovoUsuario(usuario);
        }


        [HttpGet("EnviarArquivo/{idArquivo}/{idUsuario}/{email}")]
        public void EnviarArquivo(Guid idArquivo, Guid idUsuario, string email)
        {
            service.EnviarDocumento(idArquivo, idUsuario, email);
        }

        [HttpPost("EnviarContato")]
        public async void EnviarContato([FromBody] Contato model, int? idioma)
        {
                service.SalvarContato(model);
                service.EnviarContato(model, idioma);
        }

        [HttpPost("EnviarResultadoSolar")]
        public async void EnviarResultadoSolar([FromBody] CalculoSolarResultado model)
        {
            produtoService.SalvarCalculoSolar(model);
            service.EnviarResultadoSolar(model);

        }

        [HttpPost("SejaIntegrador")]
        public async void SejaIntegrador([FromBody] FormularioIntegrador formulario)
        {
            service.EnviarSejaIntegrador(formulario);
        }

        [HttpPost("SolicitacaoRH")]
        public async void SolicitacaoRH([FromBody] FormularioSolicitacao formulario)
        {
            service.EnviarSolicitacaoRH(formulario);
        }

        [HttpPost("LojaFuncionario")]
        public async void LojaFuncionario([FromBody] FormularioLojaFuncionario formulario)
        {
            service.EnviarLojaFuncionario(formulario);
        }

        [HttpPost("SugestaoTI")]
        public async void SugestaoTI([FromBody] FormularioSugestao formulario)
        {
            service.EnviarSugestaoTI(formulario);
        }

        [HttpPost("QueroComprar")]
        public async void QueroComprar([FromBody] FormularioQueroComprar formulario)
        {
            service.EnviarQueroComprar(formulario);
        }
    }
}