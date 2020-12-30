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
    public class ImagemController : ControllerBase
    {
        private readonly ImagemService service;

        public ImagemController(IConfiguration configuration, ILogger<ImagemController> logger)
        {
            service = new ImagemService(new Config(configuration).ConnectionString, new Config(configuration).FileSettings);
        }

        [HttpGet("GetBanners/{modulo}/{componente?}")]
        public ActionResult<List<Banner>> GetBanners(string modulo, string componente = "", int? idioma = null)
        {
            return service.Banners(modulo, componente, idioma);
        }

        [HttpGet("GetImagens/{codigo}")]
        public ActionResult<List<Arquivo>> GetImagens(string codigo)
        {
            return service.ImagensPorCodigo(codigo);
        }

        [Authorize]
        [HttpPost("SalvarBanner"), DisableRequestSizeLimit]
        public ActionResult<bool> SalvarBanner([FromForm] string model)
        {
            var banner = JsonConvert.DeserializeObject<Banner>(model);

            var file = Request.Form.Files.Any() ? Request.Form.Files[0] : null;

            return service.SalvarBanner(banner, file);
      
        }
        [Authorize]
        [HttpPost("SalvarBanner2"), DisableRequestSizeLimit]
        public ActionResult<bool> SalvarBanner2([FromForm] string model)
        {
            var banner = JsonConvert.DeserializeObject<Banner>(model);

            var file = Request.Form.Files.Any() ? Request.Form.Files[0] : null;

            return service.SalvarBanner2(banner, file);

        }

        [Authorize]
        [HttpGet("RemoverBanner/{IdBanner}")]
        public ActionResult<bool> RemoverBanner(Guid IdBanner)
        {
            return service.RemoverBanner(IdBanner);
        }
    }
}