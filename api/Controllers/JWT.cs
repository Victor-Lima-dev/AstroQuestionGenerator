using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    namespace api.Controllers
    {
        [ApiController]

        [Route("api/[controller]")]
        public class JWT : ControllerBase
        {
            private readonly IConfiguration _config;

            public JWT(IConfiguration config)
            {
                _config = config;
            }

            [HttpGet("GeraToken")]
            public IActionResult GeraToken()
            {
                return Ok(GerarToken());
            }

            [Authorize]

            [HttpPost("ValidaToken")]
            public IActionResult ValidaToken()
            {
                return Ok();
            }


            private string GerarToken()
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1215645151234561321524556121653412156234152"));

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.UniqueName, "Teste"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    
                };

                // Gerar a assinatura digital do nosso token utilizando o algoritmo Hmac e a chave privada
                var credenciais = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var expiration = DateTime.UtcNow.AddHours(double.Parse("2"));

                // Gerar token
                var token = new JwtSecurityToken(
                    issuer: _config["TokenConfiguration:Issuer"],
                    audience: _config["TokenConfiguration:Audience"],
                    expires: expiration,
                    claims: claims,
                    signingCredentials: credenciais
                );

                // Retornar o token como uma string
                return new JwtSecurityTokenHandler().WriteToken(token);
            }


        }
    }
}