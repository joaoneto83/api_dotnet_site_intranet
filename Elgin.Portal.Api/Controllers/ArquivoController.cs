using Elgin.Portal.Api.Controllers.Shared;
using Elgin.Portal.Services.Implementation;
using Elgin.Portal.Services.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArquivoController : ControllerBase
    {
        private readonly ArquivoService _service;

        private readonly ILogger _logger;

        public ArquivoController(IConfiguration configuration, ILogger<ArquivoController> logger)
        {
            _service = new ArquivoService(new Config(configuration).ConnectionString,
                                          new Config(configuration).FileSettings);

            _logger = logger;
        }

        [HttpGet]
        public ActionResult<List<Arquivo>> Get()
        {
            return _service.RetornaTodos();
        }

        [HttpPost("AddArquivo")]
        public ActionResult<Arquivo> AddArquivo([FromBody] Arquivo model)
        {
            return _service.AddArquivo(model);
        }

        [HttpGet("DelArquivo/{id}")]
        public ActionResult<bool> DelArquivo(Guid id)
        {
            return _service.DelArquivo(id);
        }

        [HttpGet("GetCaminhoArquivo/{id}")]
        public ActionResult<string> GetCaminhoArquivo(Guid id)
        {
            return _service.RetornarCaminhoArquivo(id);
        }

        [HttpGet("GetTipoArquivos")]
        public ActionResult<List<TipoArquivo>> GetTipoArquivos()
        {
            return _service.RetornaTipoArquivos();
        }
    }
}