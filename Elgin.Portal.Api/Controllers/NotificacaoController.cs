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
    public class NotificacaoController : ControllerBase
    {
        private readonly NotificacaoService service;

        public NotificacaoController(IConfiguration configuration, ILogger<NotificacaoController> logger)
        {
            service = new NotificacaoService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetNotificacoes/{idUsuario}")]
        public ActionResult<List<Notificacao>> GetNotificacoes(Guid idUsuario)
        {
            return service.RetornaNotificacoes(idUsuario);
        }

        [HttpGet("VisualizarNotificacoes/{idUsuario}")]
        public void VisualizarNotificacoes(Guid idUsuario)
        {
            service.VisualizarNotificacoes(idUsuario);
        }

        [HttpGet("LimparNotificacoes/{idUsuario}")]
        public void LimparNotificacoes(Guid idUsuario)
        {
            service.LimparNotificacoes(idUsuario);
        }

    }
}