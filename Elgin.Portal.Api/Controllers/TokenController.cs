using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Elgin.Portal.Api.Controllers.Shared;
using Elgin.Portal.Services.Implementation;
using Elgin.Portal.Services.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Elgin.Portal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly UsuarioService service;
        private readonly ProvaService provaService;

        public TokenController(IConfiguration configuration, ILogger<TokenController> logger)
        {
            service = new UsuarioService(new Config(configuration).ConnectionString,
                                         new Config(configuration).FileSettings);

            provaService = new ProvaService(new Config(configuration).ConnectionString,
                                         new Config(configuration).FileSettings);
        }

        [AllowAnonymous]
        [HttpPost]
        public object Post([FromBody]User user)
        {
            bool credenciaisValidas = false;

            Usuario usuario = null;

            if (user != null && !String.IsNullOrWhiteSpace(user.UserID))
            {
                usuario = service.Login(user.UserID, user.Password);

                if (usuario != null)
                {
                    credenciaisValidas = true;
                }
            }

            if (credenciaisValidas)
            {
                new Task(() => {
                    provaService.FecharProvasFinalizadas();
                }).Start();

                var direitos =
                    new[] {
                        new Claim("id", usuario.Id.ToString()),
                        new Claim("email", usuario.Email),
                        new Claim("caminhoFoto", usuario.CaminhoFoto ?? ""),
                        new Claim("nome", usuario.Nome),
                        new Claim("sigla", usuario.Sigla),
                        new Claim("telefone", usuario.Telefone ?? string.Empty),
                        new Claim("nomeSetor", usuario.NomeSetor ?? string.Empty),
                        new Claim("registro", usuario.Registro ?? string.Empty),
                        new Claim("acessos", string.Join("|",usuario.Acessos))
                    };

                var chave = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("elgin-portal-authentication-valida"));
                var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);


                var token = new JwtSecurityToken(
                    issuer: "Elgin.Portal.Api",
                    audience: "Postman",
                    claims: direitos,
                    signingCredentials: credenciais,
                    expires: DateTime.Now.AddMinutes(180)
                    );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(tokenString);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
