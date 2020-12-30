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
    public class ClassificacaoController : ControllerBase
    {
        private readonly ClassificacaoService service;

        public ClassificacaoController(IConfiguration configuration, ILogger<ClassificacaoController> logger)
        {
            service = new ClassificacaoService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetClassificacaoPorSublinha/{codigoSublinha}")]
        public ActionResult<List<Classificacao>> GetClassificacaoPorSubLinha(string codigoSublinha, int idioma)
        {
            return service.ListarClassificacaoPorSubLinha(codigoSublinha, idioma);
        }

        [HttpGet("GetClassificacaoComparativo/{codigoSublinha}")]
        public ActionResult<List<Classificacao>> GetClassificacaoComparativo(string codigoSublinha, int idioma = 1)
        {
            return service.ListarClassificacaoPorComparativo(codigoSublinha, idioma);
        }
    }
}