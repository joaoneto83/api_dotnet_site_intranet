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
    [Authorize]
    public class GrupoController : ControllerBase
    {
        private readonly GrupoService service;

        public GrupoController(IConfiguration configuration, ILogger<GrupoController> logger)
        {
            service = new GrupoService(new Config(configuration).ConnectionString, new Config(configuration).FileSettings);
        }

        [HttpGet("GetGrupos/{termo?}")]
        public ActionResult<List<Grupo>> GetGrupos(string termo)
        {
            return service.ListarGrupos(termo);
        }

        [HttpGet("GetGrupoPorId/{idGrupo}")]
        public ActionResult<Grupo> GetGrupoPorId(Guid idGrupo)
        {
            return service.ProcuraGrupoPorId(idGrupo);
        }

        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar(Grupo grupo)
        {
            return service.Salvar(grupo);
        }

        [HttpGet("InativarGrupo/{idGrupo}")]
        public ActionResult<bool> InativarGrupo(Guid idGrupo)
        {
            return service.InativarGrupo(idGrupo);
        }
    }
}