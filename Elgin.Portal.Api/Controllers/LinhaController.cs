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
    public class LinhaController : ControllerBase
    {
        private readonly ILogger<LinhaController> _logger;
        private readonly LinhaService service;

        public LinhaController(IConfiguration configuration, ILogger<LinhaController> logger)
        {
            _logger = logger;
            service = new LinhaService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetLinhas")]
        public ActionResult<List<Linha>> GetLinhas(int codigoIdioma = 1)
        {
            return service.ListarLinhas(codigoIdioma);
        }

        [HttpGet("GetLinhasComSubLinha/{possuiMenusEspeciais?}")]
        public ActionResult<List<Linha>> GetLinhasComSubLinha(bool possuiMenusEspeciais = false)
        {
            return service.ListarLinhasComSublinha(possuiMenusEspeciais);
        }

        [HttpGet("GetLinhasComTodasSubLinhas")]
        public ActionResult<List<Linha>> GetLinhasComTodasSubLinhas()
        {
            return service.ListarLinhasComTodasSublinhas();
        }
    }
}