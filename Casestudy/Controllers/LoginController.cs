using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Casestudy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly IConfiguration _configuration;

        public LoginController(AppDbContext ctx, IConfiguration config)
        {
            _ctx = ctx;
            _configuration = config;
        }

        [HttpPost]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerHelper>> Login(CustomerHelper helper)
        {
            CustomerDAO dao = new(_ctx);
            Customer? customer = await dao.GetByEmail(helper.Email!);
            if (customer != null)
            {
                if (VerifyPassword(helper.Password, customer.Hash!, customer.Salt!))
                {
                    helper.Password = "";
                    var appSettings = _configuration.GetSection("AppSettings").GetValue<string>("Secret");
                    var key = Encoding.ASCII.GetBytes(appSettings!);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, customer.Id.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    helper.Token = tokenHandler.WriteToken(token);
                }
                else
                {
                    helper.Token = "invalid credentials - login failed";
                }
            }
            else
            {
                helper.Token = "no such customer - login failed";
            }
            return helper;
        }

        public static bool VerifyPassword(string? enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            using var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword ?? "", saltBytes, 10000, HashAlgorithmName.SHA256);
            var enteredHash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
            return enteredHash == storedHash;
        }
    }
}