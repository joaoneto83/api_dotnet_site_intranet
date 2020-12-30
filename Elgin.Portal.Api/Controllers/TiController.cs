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
    public class TiController : ControllerBase
    {
        private readonly TiService service;

        public TiController(IConfiguration configuration, ILogger<TiController> logger)
        {
            service = new TiService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetModulosArquivos")]
        public ActionResult<List<ModuloArquivoTi>> GetTabelasPreco()
        {
            return service.ListarModuloArquivo();
        }

        [HttpPost("SalvarModulo")]
        public ActionResult<bool> Salvar([FromBody] ModuloArquivoTi model)
        {
            return service.SalvarModulo(model);
        }
    }
}