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
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService service;

        public UsuarioController(IConfiguration configuration, ILogger<UsuarioController> logger)
        {
            service = new UsuarioService(new Config(configuration).ConnectionString,
                                         new Config(configuration).EmailSettings,
                                         new Config(configuration).FileSettings);
        }

        [Authorize]
        [HttpGet()]
        public async Task<List<Usuario>> Get()
        {
            return this.service.Listar();
        }

        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<Usuario> Get(Guid id)
        {
            return service.ProcurarPorId(id);
        }

        [Authorize]
        [HttpGet("GetAniversariantesMes")]
        public ActionResult<List<Usuario>> GetAniversariantesMes()
        {
            return service.ListarAniversariantesMes();
        }


        [HttpGet("GetListaTelefonica/{letra?}")]
        public ActionResult<List<Usuario>> GetListaTelefonica(string letra)
        {
            return service.ListaTelefonica(letra);
        }

        [Authorize]
        [HttpGet("GetUsuarios/{termoPesquisado?}")]
        public ActionResult<List<Usuario>> GetUsuarios(string termoPesquisado)
        {
            return service.ListarUsuarios(termoPesquisado);
        }

        [HttpGet("ResetarSenha/{login}")]
        public ActionResult<bool> ResetarSenha(string login)
        {
            return service.ResetarSenha(login);
        }

        [HttpGet("ValidarCodigo/{login}/{codigo}")]
        public ActionResult<Guid> ValidarCodigo(string login, string codigo)
        {
            return service.ValidarCodigo(login, codigo);
        }

        [HttpPost("AlterarSenha")]
        public ActionResult<bool> AlterarSenha(NovaSenha model)
        {
            return service.AlterarSenha(model);
        }

        [HttpPost("ValidaLogin")]
        public ActionResult<bool> ValidaLogin(Usuario model)
        {
            return service.ValidaLogin(model);
        }

        //[Authorize]
        [HttpPost("Salvar"), DisableRequestSizeLimit]
        public async Task<ActionResult<bool>> Salvar([FromForm]string model, [FromForm]string caminho, [FromForm]string nomeArquivo, [FromForm]string idPai)
        {
            var usuario = JsonConvert.DeserializeObject<Usuario>(model);

            var file = Request.Form.Files.Any() ? Request.Form.Files[0] : null;

            return await service.SalvarAsync(usuario, file, caminho, nomeArquivo, idPai);
        }
    }
}