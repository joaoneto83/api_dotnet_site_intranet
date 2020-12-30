using System;
using System.Collections.Generic;
using Elgin.Portal.Api.Controllers.Shared;
using Elgin.Portal.Services.Implementation;
using Elgin.Portal.Services.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _service;
        private readonly ArquivoService _arquivoService;

        public ProdutoController(IConfiguration configuration, ILogger<ProdutoController> logger)
        {
            _service = new ProdutoService(new Config(configuration).ConnectionString,
                                         new Config(configuration).FileSettings);
            _arquivoService = new ArquivoService(new Config(configuration).ConnectionString,
                                         new Config(configuration).FileSettings);
        }

        [HttpGet]
        public ActionResult<Produto> Get(string produto, bool? ativo, int? idioma = 1)
        {
            return _service.PreencheProduto(produto, ativo, idioma);
        }

        [HttpGet("GetProdutosFiltro/{codigo}")]
        public ActionResult<List<Produto>> GetProdutosFiltro(string codigo, int idioma = 1)
        {
            return _service.RetornaProdutoFiltro(codigo, idioma);
        }

        [HttpGet("GetProdutosFiltroPorCodigoLinha/{codigoLinha}")]
        public ActionResult<List<Produto>> GetProdutosFiltroPorCodigoLinha(string codigoLinha)
        {
            return _service.RetornaProdutoPorCodigoLinha(codigoLinha);
        }

        [HttpGet("GetProdutosLinha/{id}")]
        public ActionResult<List<Produto>> GetProdutosLinha(Guid id)
        {
            return _service.RetornaProdutosPorLinha(id);
        }

        [HttpGet("GetProdutosSubLinha/{id}")]
        public ActionResult<List<Produto>> GetProdutosSubLinha(Guid id, int? idioma)
        {
            return _service.RetornaProdutosPorSubLinha(id, idioma);
        }

        [HttpGet("GetProdutos/{termoPesquisado?}")]
        public ActionResult<List<Produto>> GetProdutos(string termoPesquisado)
        {
            return _service.RetornaTodosProdutos(termoPesquisado);
        }

        [HttpGet("GetResultadoBusca/{texto}")]
        public ActionResult<List<ResultadoInputBusca>> GetResultadoBusca(string texto)
        {
            return _service.PesquisaProdutos(texto);
        }

        [HttpGet("GetBuscaProdutos/{texto}")]
        public ActionResult<List<ResultadoInputBusca>> GetBuscaProdutos(string texto)
        {
            return _service.PesquisaProdutosDetalhes(texto);
        }

        [Authorize]
        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar([FromBody] Produto model)
        {
            return _service.Salvar(model);
        }


        [HttpPost("AparelhoIdeal")]
        public ActionResult<List<AparelhoIdealResultado>> AparelhoIdeal([FromBody] AparelhoIdeal model )
        {
            return _service.RetornaAparelhoIdeal(model);
        }

        [Authorize]
        [HttpPost("CopiarProduto")]
        public ActionResult<bool> CopiarProduto([FromForm]string produto, [FromForm]string idProdutoDestino)
        {
            var objProduto = JsonConvert.DeserializeObject<Produto>(produto);

            var idDestino = JsonConvert.DeserializeObject<string>(idProdutoDestino);

            return _service.CopiarProduto(objProduto, idDestino);
        }

        [HttpPost("CalculoSolar")]
        public ActionResult<CalculoSolarResultado> CalculoSolar([FromBody] CalculoSolar model)
        {
            return _service.RetornaCalculoSolar(model);
        }

        [Authorize]
        [HttpGet("Secoes")]
        public ActionResult<List<Secao>> Secoes()
        {
            return _service.RetornaSecoes();
        }

        [Authorize]
        [HttpPost("AddSecaoProduto")]
        public ActionResult<SecaoProduto> AddSecaoProduto( [FromBody] SecaoProduto model )
        {
            return _service.AddSecaoProduto(model);
        }

        [Authorize]
        [HttpDelete("DelSecoesProduto/{id}")]
        public ActionResult<bool> DelSecoesProduto(string id)
        {
            return _service.DelSecaoProduto(id);
        }

        [Authorize]
        [HttpPost("AddImagemSecaoProduto")]
        public ActionResult<Arquivo> AddImagemSecaoProduto([FromBody] Arquivo model)
        {
            return _service.AddImagemSecaoProduto(model);
        }

        [Authorize]
        [HttpDelete("DelImagemSecaoProduto/{id}")]
        public ActionResult<bool> DelImagemSecaoProduto(Guid id)
        {
            return _arquivoService.DelArquivo(id);
        }

        [Authorize]
        [HttpGet("SecoesProduto/{idProduto}")]
        public ActionResult<List<SecaoProduto>> SecoesProduto(Guid idProduto)
        {
            return _service.RetornaSecoesProduto(idProduto, null, null);
        }

        [Authorize]
        [HttpPost("SalvarNovoProduto")]
        public Produto SalvarNovoProduto(Produto produto)
        {
            return _service.SalvarNovoProduto(produto);
        }
        [Authorize]
        [HttpPost("AddSecaoModelo")]
        public ActionResult<SecaoModelo> AddSecaoModelo([FromBody] SecaoModelo model)
        {
            return _service.AddSecaoModelo(model);
        }

        [Authorize]
        [HttpDelete("DelSecoesModelo/{id}")]
        public ActionResult<bool> DelSecoesModelo(string id)
        {
            return _service.DelSecaoModelo(id);
        }

        [Authorize]
        [HttpPost("AddImagemSecaoModelo")]
        public ActionResult<Arquivo> AddImagemSecaoModelo([FromBody] Arquivo model)
        {
            return _service.AddImagemSecaoModelo(model);
        }

        [Authorize]
        [HttpDelete("DelImagemSecaoModelo/{id}")]
        public ActionResult<bool> DelImagemSecaoModelo(Guid id)
        {
            return _arquivoService.DelArquivo(id);
        }

        [Authorize]
        [HttpGet("SecoesModelo/{IdSecaoModeloGrupo}")]
        public ActionResult<List<SecaoModelo>> SecoesModelo(Guid IdSecaoModeloGrupo)
        {
            return _service.RetornaSecoesModelo(IdSecaoModeloGrupo, null);
        }

        [Authorize]
        [HttpGet("SecoesModeloGrupo/")]
        public ActionResult<List<SecaoModeloGrupo>> SecoesModeloGrupo()
        {
            return _service.RetornaSecoesModeloGrupo();
        }

        [Authorize]
        [HttpGet("SecoesModeloGrupo/{IdSecaoModeloGrupo}")]
        public ActionResult<SecaoModeloGrupo> SecoesModeloGrupo(Guid IdSecaoModeloGrupo)
        {
            return _service.RetornaSecoesModeloGrupo(IdSecaoModeloGrupo);
        }

        [Authorize]
        [HttpPost("AddSecaoModeloGrupo")]
        public ActionResult<SecaoModeloGrupo> AddSecaoModeloGrupo([FromBody] SecaoModeloGrupo model)
        {
            return _service.AddSecaoModeloGrupo(model);
        }

        [Authorize]
        [HttpPost("UpdateSecaoModelo")]
        public ActionResult<SecaoModelo> UpdateSecaoModeloGrupo([FromBody] SecaoModelo model)
        {
            _service.AtualizarSecoesModelo(model);
            return model;
        }

        //[Authorize]
        [HttpPost("SalvarSecaoModeloGrupo")]
        public ActionResult<SecaoModeloGrupo> SalvarSecaoModeloGrupo([FromBody] SecaoModeloGrupo model)
        {
            _service.SalvarSecaoModeloGrupo(model);
            return model;
        }

        //[Authorize]
        [HttpGet("RetornaSecoesModeloGrupoProduto/{IdSecaoModeloGrupo}")]
        public ActionResult<List<Produto>> RetornaSecoesModeloGrupoProduto(Guid IdSecaoModeloGrupo)
        {
            List<Produto> model = new List<Produto>();
            model = _service.RetornaSecoesModeloGrupoProduto(IdSecaoModeloGrupo);

            foreach(var i in model)
            {
                i.Ativo = true;
            }

            return model;
        }

        [Authorize]
        [HttpGet("GetProdutosPorCodigoLinha/{codigoLinha}")]
        public ActionResult<List<Produto>> GetProdutosPorCodigoLinha(string codigoLinha)
        {
            return _service.RetornarProdutosPorCodigoLinha(codigoLinha);
        }

    }
}