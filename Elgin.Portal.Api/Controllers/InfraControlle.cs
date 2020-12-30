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
    public class InfraController : ControllerBase
    {
        private readonly InfraService service;
        private readonly ProdutoService produtoService;

        public InfraController(IConfiguration configuration, ILogger<InfraController> logger)
        {
            service = new InfraService(new Config(configuration).ConnectionString);
        }
       
        [HttpPost("Log")]
        public async void Log( [FromBody] Log model)
        {
            service.GravaLog(model);
        }

    }
}