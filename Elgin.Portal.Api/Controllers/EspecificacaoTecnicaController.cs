using System;
using System.Collections.Generic;
using Elgin.Portal.Api.Controllers.Shared;
using Elgin.Portal.Services.Implementation;
using Elgin.Portal.Services.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EspecificacaoTecnicaController : ControllerBase
    {
        private readonly EspecificacaoTecnicaService _service;

        public EspecificacaoTecnicaController(IConfiguration configuration, ILogger<EspecificacaoTecnicaController> logger)
        {
            _service = new EspecificacaoTecnicaService(new Config(configuration).ConnectionString);
        }
        
        [HttpGet("ListarEspecificacoesTecnicas")]
        public ActionResult<List<EspecificacaoTecnica>> ListarEspecificacoesTecnicas(Guid idSublinha, Guid idProduto)
        {
            return _service.ListarEspecificacoesTecnicas(idSublinha, idProduto, null, null);
        }

        [HttpGet("ListarEspecificacoesTecnicasComparativo")]
        public ActionResult<List<EspecificacaoTecnica>> ListarEspecificacoesTecnicasComparativo(string codigoSublinha, int idioma = 1)
        {
            return _service.ListarEspecificacoesTecnicas(codigoSublinha, idioma);
        }

    }
}