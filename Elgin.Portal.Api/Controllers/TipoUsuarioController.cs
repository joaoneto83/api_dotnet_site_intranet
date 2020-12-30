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
using Newtonsoft.Json;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoUsuarioController : ControllerBase
    {
        private readonly TipoUsuarioService service;

        public TipoUsuarioController(IConfiguration configuration, ILogger<TipoUsuarioController> logger)
        {
            service = new TipoUsuarioService(new Config(configuration).ConnectionString);
        }

        [Authorize]
        [HttpGet("Listar")]
        public ActionResult<List<TipoUsuario>> Listar()
        {
            return service.Listar();
        }

    }
}