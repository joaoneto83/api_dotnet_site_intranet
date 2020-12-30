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
    public class SubLinhaController : ControllerBase
    {
        private readonly SubLinhaService service;

        public SubLinhaController(IConfiguration configuration, ILogger<SubLinhaController> logger)
        {
            service = new SubLinhaService(new Config(configuration).ConnectionString, new Config(configuration).FileSettings);
        }

        [HttpGet("GetSubLinhasPorLinha/{idLinha}")]
        public ActionResult<List<SubLinha>> GetSubLinhasPorLinha(Guid idLinha)
        {
            return service.ListarSubLinhas(idLinha);
        }

        [HttpGet("GetSubLinhasPorCodigoLinha/{codigoLinha}")]
        public ActionResult<List<SubLinha>> GetSubLinhasPorCodigoLinha(string codigoLinha, int? idioma)
        {
            return service.ListarSubLinhasCodigoLinha(codigoLinha, idioma);
        }

        [HttpGet("GetSubLinhaPorCodigo/{codigo}")]
        public ActionResult<SubLinha> GetSubLinhaPorCodigo(string codigo, int idioma = 1)
        {
            return service.ProcurarPorCodigo(codigo, idioma);
        }

        [HttpGet("GetSubLinhasProdutosPorCodigo/{codigo}")]
        public ActionResult<List<SubLinha>> GetSubLinhasProdutosPorCodigo(string codigo)
        {
            return service.ListarSubLinhasProdutos(codigo);
        }

        [Authorize]
        [HttpGet("GetSubLinha/{id}")]
        public ActionResult<SubLinha> GetSubLinha(Guid id)
        {
            return service.PreencheSubLinha(id);
        }

        [Authorize]
        [HttpPost("Salvar")]
        public ActionResult<SubLinha> Salvar([FromBody] SubLinha model)
        {
            return service.Salvar(model);
        }
    }
}