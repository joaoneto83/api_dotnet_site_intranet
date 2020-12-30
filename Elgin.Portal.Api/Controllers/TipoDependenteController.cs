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
    public class TipoDependenteController : ControllerBase
    {
        private readonly TipoDependenteService service;

        public TipoDependenteController(IConfiguration configuration, ILogger<TipoDependenteController> logger)
        {
            service = new TipoDependenteService(new Config(configuration).ConnectionString);
        }

        [Authorize]
        [HttpGet("Listar")]
        public ActionResult<List<TipoDependente>> Listar()
        {
            return service.Listar();
        }

    }
}