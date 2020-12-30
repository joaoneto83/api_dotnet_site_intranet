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
    //[Authorize]
    public class SetorController : ControllerBase
    {
        private readonly SetorService service;

        public SetorController(IConfiguration configuration, ILogger<SetorController> logger)
        {
            service = new SetorService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetSetores")]
        public ActionResult<List<Setor>> GetSetores()
        {
            return service.Listar();
        }

        [HttpGet("GetSetor/{id}")]
        public ActionResult<Setor> GetSetor(Guid id)
        {
            return service.RetornarSetorPorId(id);
        }
    }
}