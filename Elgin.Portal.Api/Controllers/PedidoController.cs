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
    public class PedidoController : ControllerBase
    {
        private readonly PedidoService service;

        public PedidoController(IConfiguration configuration, ILogger<PecaReposicaoController> logger)
        {
            service = new PedidoService(new Config(configuration).ConnectionString, new Config(configuration).EmailSettings, new Config(configuration).FileSettings);
        }

        [HttpPost("Salvar")]
        public ActionResult<int> Salvar(Pedido pedido)
        {
            return service.Salvar(pedido);
        }


    }
}