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
    public class DenunciaController : ControllerBase
    {
        private readonly DenunciaService service;

        public DenunciaController(IConfiguration configuration, ILogger<DenunciaController> logger)
        {
            service = new DenunciaService(new Config(configuration).ConnectionString,
                                         new Config(configuration).EmailSettings,
                                         new Config(configuration).FileSettings);
        }

  

        [HttpGet("Listar")]
        public ActionResult<List<Denuncia>> Listar()
        {
            return service.Listar();
        }

        [HttpPost("enviarEmail")]
        public void enviarEmail([FromForm] string model)
        {
            var denuncia = JsonConvert.DeserializeObject<Denuncia>(model);

            service.enviarEmail(denuncia);
        }


        [HttpGet("{protocolo}")]
        public ActionResult<Denuncia> Get(string protocolo)
        {
            return service.ProcurarPorProtocolo(protocolo);
        }



        [HttpPost]
        public ActionResult<bool> Salvar([FromBody] Denuncia model)
        {
            return service.Salvar(model);
        }


        //[HttpPost("Salvar"), DisableRequestSizeLimit]
        //public async Task<ActionResult<bool>> Salvar([FromForm]string model, [FromForm]string caminho, [FromForm]string nomeArquivo, [FromForm]string idPai)
        //{
        //    var denuncia = JsonConvert.DeserializeObject<Denuncia>(model);

        //    var file = Request.Form.Files.Any() ? Request.Form.Files[0] : null;

        //    return await service.SalvarAsync(denuncia, file, caminho, nomeArquivo, idPai);
        //}
    }
}