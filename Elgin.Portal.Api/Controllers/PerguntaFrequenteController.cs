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
using Newtonsoft.Json.Serialization;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerguntaFrequenteController : ControllerBase
    {
        private readonly PerguntaFrequenteService service;

        public PerguntaFrequenteController(IConfiguration configuration, ILogger<PerguntaFrequenteController> logger)
        {
            service = new PerguntaFrequenteService(new Config(configuration).ConnectionString);
        }

        [HttpGet("ListarPorIdProduto/{codigo}")]
        public ActionResult<List<PerguntaFrequente>> ListarPorIdProduto(string codigo)
        {
            return service.ListarPorCodigoComponente(codigo);
        }

    }
}