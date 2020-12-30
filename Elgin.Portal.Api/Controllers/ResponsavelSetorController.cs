using System.Collections.Generic;
using Elgin.Portal.Api.Controllers.Shared;
using Elgin.Portal.Services.Implementation;
using Elgin.Portal.Services.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ResponsavelSetorController : ControllerBase
    {
        private readonly ILogger<ResponsavelSetorController> _logger;
        private readonly ResponsavelSetorService service;

        public ResponsavelSetorController(IConfiguration configuration, ILogger<ResponsavelSetorController> logger)
        {
            _logger = logger;
            service = new ResponsavelSetorService(new Config(configuration).ConnectionString);
        }

        [HttpGet]
        public ActionResult<List<ResponsavelSetor>> Get()
        {
            return service.Listar();
        }

        [HttpPost("Salvar")]
        public ActionResult<bool> Salvar([FromBody] ResponsavelSetor model)
        {
            return service.Salvar(model);
        }
    }
}