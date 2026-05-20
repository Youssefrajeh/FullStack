using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Casestudy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public RegisterController(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpPost]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerHelper>> Register(CustomerHelper helper)
        {
            CustomerDAO dao = new(_ctx);
            Customer? already = await dao.GetByEmail(helper.Email!);
            if (already == null)
            {
                HashSalt hashSalt = GenerateSaltedHash(64, helper.Password!);
                helper.Password = "";
                Customer dbCustomer = new()
                {
                    Firstname = helper.Firstname!,
                    Lastname = helper.Lastname!,
                    Email = helper.Email!,
                    Hash = hashSalt.Hash!,
                    Salt = hashSalt.Salt!
                };
                dbCustomer = await dao.Register(dbCustomer);
                helper.Token = dbCustomer.Id > 0 ? "customer registered" : "registration failed";
            }
            else
            {
                helper.Token = "registration failed - email already in use";
            }
            return helper;
        }

        private static HashSalt GenerateSaltedHash(int size, string password)
        {
            var saltBytes = new byte[size];
            using var provider = RandomNumberGenerator.Create();
            provider.GetNonZeroBytes(saltBytes);

            using var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password ?? "", saltBytes, 10000, HashAlgorithmName.SHA256);
            var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));

            return new HashSalt { Hash = hashPassword, Salt = Convert.ToBase64String(saltBytes) };
        }
    }
}