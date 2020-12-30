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
    public class VideoController : ControllerBase
    {
        private readonly VideoService service;

        public VideoController(IConfiguration configuration, ILogger<VideoController> logger)
        {
            service = new VideoService(new Config(configuration).ConnectionString);
        }

        [HttpGet("GetVideos/{modulo}")]
        public ActionResult<List<Video>> GetVideos(string modulo, int? idioma)
        {
            return service.ListarVideos(modulo, idioma);
        }

        [Authorize]
        [HttpPost("Salvar"), DisableRequestSizeLimit]
        public ActionResult<List<Video>> Salvar([FromBody] List<Video> videos)
        {
            return service.Salvar(videos);
        }
    }
}