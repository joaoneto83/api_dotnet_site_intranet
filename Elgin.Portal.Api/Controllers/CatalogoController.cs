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
    public class CatalogoController : ControllerBase
    {
        private readonly CatalogoService _service;

        public CatalogoController(IConfiguration configuration, ILogger<CatalogoController> logger)
        {
            _service = new CatalogoService(new Config(configuration).ConnectionString,
                                         new Config(configuration).FileSettings);
        }

        [HttpGet("GetCatalogos")]
        public ActionResult<List<Catalogo>> GetCatalogos()
        {
            return _service.RetornaCatalogo();
        }

        [HttpPost("PostArquivoCatalogo")]
        public bool PostArquivoCatalogo(Arquivo arquivo)
        {
            return _service.SalvarArquivoCatalogo(arquivo);
        }
    }
}