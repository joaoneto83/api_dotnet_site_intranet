using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Elgin.Portal.Api.Controllers.Shared;
using Elgin.Portal.Services.Implementation;
using Elgin.Portal.Services.Implementation.Shared;
using Elgin.Portal.Services.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace Elgin.Portal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly ArquivoService arquivoService;

        private IHostingEnvironment _hostingEnvironment;

        public FileController(IHostingEnvironment hostingEnvironment, IConfiguration configuration, ILogger<FileController> logger)
        {
            arquivoService = new ArquivoService(new Config(configuration).ConnectionString,
                                                new Config(configuration).FileSettings);
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost("Post"), DisableRequestSizeLimit]
        public async Task<ActionResult<FileResponse>> Post([FromForm] string caminho, [FromForm]string nomeArquivo, [FromForm]string idPai)
        {

            var file = Request.Form.Files[0];

            return await arquivoService.SalvarArquivo(file, caminho, nomeArquivo, idPai);
        }
    }
}