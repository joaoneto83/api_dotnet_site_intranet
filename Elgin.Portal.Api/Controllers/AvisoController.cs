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
    public class AvisoController : ControllerBase
    {
        private readonly AvisoService service;
        

        public AvisoController(IConfiguration configuration, ILogger<AvisoController> logger)
        {
            service = new AvisoService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetAvisosPorModulo/{modulo}")]
        public ActionResult<List<Aviso>> GetAvisosPorModulo(string modulo)
        {
            return service.ListarPorModulo(modulo);
        }

        [HttpGet("GetAvisoPorId/{id}")]
        public ActionResult<Aviso> GetAvisoPorId(Guid id)
        {
            return service.ProcurarAvisoPorId(id);
        }

        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar([FromBody] Aviso model)
        {
            return service.Salvar(model);
        }
    }
}