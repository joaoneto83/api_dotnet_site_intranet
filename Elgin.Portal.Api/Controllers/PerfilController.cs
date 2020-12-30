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
    public class PerfilController : ControllerBase
    {
        private readonly PerfilService service;

        public PerfilController(IConfiguration configuration, ILogger<PerfilController> logger)
        {
            service = new PerfilService(new Config(configuration).ConnectionString);
        }

        [HttpGet("Get")]
        public ActionResult<List<Perfil>> Get()
        {
            return service.Listar();
        }

        [HttpGet("Get/{id}")]
        public ActionResult<Perfil> Get(Guid id)
        {
            return service.ProcurarPorId(id);
        }

        [HttpGet("GetMenusFuncionalidades/{idPerfil}")]
        public ActionResult<List<Menu>> MenusFuncionalidades(Guid idPerfil)
        {
            return service.ListarMenusFuncionalidades(idPerfil);
        }

        [HttpPost()]
        public ActionResult<bool> Post([FromBody] Perfil model)
        {
            return service.Salvar(model);
        }

        [HttpGet("GetMenu/{id}")]
        public ActionResult<List<Menu>> GetMenu(Guid id)
        {
            return service.RetornaMenu(id);
        }
    }
}