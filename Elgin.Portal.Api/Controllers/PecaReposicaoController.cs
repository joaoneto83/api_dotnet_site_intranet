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
using Newtonsoft.Json.Serialization;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PecaReposicaoController : ControllerBase
    {
        private readonly PecaReposicaoService service;

        public PecaReposicaoController(IConfiguration configuration, ILogger<PecaReposicaoController> logger)
        {
            service = new PecaReposicaoService(new Config(configuration).ConnectionString, new Config(configuration).FileSettings);
        }

        [Authorize]
        [HttpGet("ListarPorIdProduto/{idProduto}")]
        public ActionResult<string> ListarPorIdProduto(Guid idProduto)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var model = JsonConvert.SerializeObject(new { lista = service.ListarPorIdProduto(idProduto), arquivo = service.GetArquivo(idProduto) }, serializerSettings);

            return model;
        }

        [Authorize]
        [HttpPost("Salvar"), DisableRequestSizeLimit]
        public ActionResult<bool> Salvar([FromForm]string pecas, [FromForm]string arquivo, [FromForm]Guid idProduto)
        {
            var arquivoPeca = JsonConvert.DeserializeObject<Arquivo>(arquivo);
            var pecasReposicao = JsonConvert.DeserializeObject<List<PecaReposicao>>(pecas);

            return service.Salvar(pecasReposicao, arquivoPeca, idProduto);
        }


        [HttpGet("ListarPorCodigoProduto/{codigo}")]
        public ActionResult<string> ListarPorCodigoProduto(string codigo)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();


            var model = JsonConvert.SerializeObject(new { lista = service.ListarPorCodigoProduto(codigo), arquivo = service.GetArquivo(codigo) }, serializerSettings);

            return model;
        }

    }
}