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
    public class GrupoDestaqueController : ControllerBase
    {
        private readonly GrupoDestaqueService service;

        public GrupoDestaqueController(IConfiguration configuration, ILogger<GrupoDestaqueController> logger)
        {
            service = new GrupoDestaqueService(new Config(configuration).ConnectionString);
        }

        [Authorize]
        [HttpGet("GetGruposDestaque/{modulo}")]
        public ActionResult<List<GrupoDestaque>> GetGruposDestaque(string modulo)
        {
            return service.ListarGrupos(modulo, null);
        }

        [HttpGet("GetGrupoDestaqueProduto/{modulo}")]
        public ActionResult<List<GrupoDestaque>> GetGrupoDestaqueProduto(string modulo, int idioma)
        {
            return service.RetornarGrupoDestaqueProdutos(modulo, idioma);
        }

        [HttpGet("GetGrupoDestaquePorCodigo/{codigo}")]
        public ActionResult<GrupoDestaque> GetGrupoDestaquePorCodigo(string codigo, int idioma = 1)
        {
            return service.RetornarGrupoDestaquePorCodigo(codigo, idioma);
        }

        [HttpGet("GetLinksGrupoDestaque")]
        public ActionResult<LinksGrupoDestaque> GetLinksGrupoDestaque()
        {
            return service.RetornarLinksGrupoDestaque();
        }

    }
}