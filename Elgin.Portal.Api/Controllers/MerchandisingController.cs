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
    [Authorize]
    public class MerchandisingController : ControllerBase
    {
        private readonly MerchandisingService _service;

        public MerchandisingController(IConfiguration configuration, ILogger<MerchandisingController> logger)
        {
            _service = new MerchandisingService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetMerchandising")]
        public ActionResult<Merchandising> GetMerchandising()
        {
            return _service.GetMerchandising();
        }

        [HttpGet("GetArquivosMpdv")]
        public ActionResult<List<Arquivo>> GetArquivosMpdv()
        {
            return _service.ListaArquivosMpdv();
        }

        [HttpGet("GetPastaTreinamentoMerchan/{id?}")]
        public ActionResult<ResponsePastaTreinamentoMerchan> GetPastaTreinamentoMerchan(Guid? id)
        {
            return _service.ListarPastaTreinamento(id);
        }

        [HttpPost("SalvarPasta")]
        public ActionResult<bool> SalvarPasta([FromBody] PastaTreinamentoMerchan model)
        {
            return _service.SalvarPasta(model);
        }

        [HttpPost("SalvarTreinamento")]
        public ActionResult<bool> SalvarTreinamento([FromBody] TreinamentoMerchan model)
        {
            return _service.SalvarTreinamento(model);
        }

        [HttpGet("ListarPastas")]
        public ActionResult<List<PastaTabelaPreco>> ListarPastas()
        {
            return _service.ListarAllPastas();
        }

        [HttpGet("MoverItem/{id}/{idPastaSelecionada}/{isTreinamento}")]
        public ActionResult<bool> MoverItem(Guid id, Guid idPastaSelecionada, bool isTreinamento)
        {
            return _service.MoverItem(id, idPastaSelecionada, isTreinamento);
        }
    }
}