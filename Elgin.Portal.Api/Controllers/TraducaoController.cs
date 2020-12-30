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
    //[Authorize]
    public class TraducaoController : ControllerBase
    {
        private readonly TraducaoService service;

        public TraducaoController(IConfiguration configuration, ILogger<TraducaoController> logger)
        {
            service = new TraducaoService(new Config(configuration).ConnectionString);
        }

        [HttpGet("Sublinha/{idSublinha}")]
        public ActionResult<List<Traducao>> Sublinha(Guid idSublinha)
        {
            return service.Sublinha(idSublinha);
        }

        [HttpGet("Produto/{idProduto}")]
        public ActionResult<List<Traducao>> Produto(Guid idProduto)
        {
            return service.Produto(idProduto);
        }

        [HttpGet("Banner/{codigoBanner}")]
        public ActionResult<List<Traducao>> Banner(string codigoBanner)
        {
            return service.Banner(codigoBanner);
        }

        [HttpGet("Refrigeracao/{id}")]
        public ActionResult<List<Traducao>> Refrigeracao(string id)
        {
            return service.Refrigeracao();
        }

        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar([FromBody] List<Traducao> model)
        {
            return service.Salvar(model);
        }
    }
}