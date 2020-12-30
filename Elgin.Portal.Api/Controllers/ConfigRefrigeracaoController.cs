using System.Collections.Generic;
using Elgin.Portal.Api.Controllers.Shared;
using Elgin.Portal.Services.Implementation;
using Elgin.Portal.Services.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigRefrigeracaoController : ControllerBase
    {
        private readonly VideoService videoService;
        private readonly GrupoDestaqueService grupoDestaqueService;

        public ConfigRefrigeracaoController(IConfiguration configuration, ILogger<ConfigRefrigeracaoController> logger)
        {
            videoService = new VideoService(new Config(configuration).ConnectionString);
            grupoDestaqueService = new GrupoDestaqueService(new Config(configuration).ConnectionString);
        }

        [Authorize]
        [HttpPost("Salvar"), DisableRequestSizeLimit]
        public ActionResult<List<Video>> Salvar([FromForm]string videos, [FromForm]string grupos, [FromForm]string links)
        {
            var modelVideos = JsonConvert.DeserializeObject<List<Video>>(videos);
            var modelGrupos = JsonConvert.DeserializeObject<List<GrupoDestaque>>(grupos);
            var modelLinks = JsonConvert.DeserializeObject<LinksGrupoDestaque>(links);

            if (grupoDestaqueService.Salvar(modelGrupos) && grupoDestaqueService.SalvarLinksGrupoDestaque(modelLinks))
                return videoService.Salvar(modelVideos);

            return null;
        }
    }
}