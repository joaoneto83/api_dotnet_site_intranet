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
    
    public class TabelaPrecoController : ControllerBase
    {
        private readonly TabelaPrecoService service;

        public TabelaPrecoController(IConfiguration configuration, ILogger<TabelaPrecoController> logger)
        {
            service = new TabelaPrecoService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetTabelasPreco")]
        public ActionResult<List<TabelaPreco>> GetTabelasPreco()
        {
            return service.ListarTabelasPreco();
        }

        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar([FromBody] TabelaPreco model)
        {
            return service.Salvar(model);
        }

        [HttpPost("SalvarPasta")]
        public ActionResult<bool> SalvarPasta([FromBody] PastaTabelaPreco model)
        {
            return service.SalvarPasta(model);
        }

        [HttpGet("GetPastaTabelaPreco/{id?}")]
        public ActionResult<ResponsePastaTabelaPreco> GetPastaTabelaPreco(Guid? id)
        {
            return service.ListarPastaTabelaPreco(id);
        }

        [HttpGet("ListarPastas")]
        public ActionResult<List<PastaTabelaPreco>> ListarPastas()
        {
            return service.ListarAllPastas();
        }

        [HttpGet("MoverItem/{id}/{idPastaSelecionada}/{isTabela}")]
        public ActionResult<bool> MoverItem(Guid id, Guid idPastaSelecionada, bool isTabela)
        {
            return service.MoverItem(id, idPastaSelecionada, isTabela);
        }
    }
}