using Casestudy.DAL;
using Casestudy.DAL.DomainClasses;
using Casestudy.DAL.DAO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Temporarily commented out for development
    public class BrandController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public BrandController(AppDbContext context)
        {
            _ctx = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Brand>>> Index()
        {
            BrandsDAO dao = new(_ctx);
            List<Brand> allBrands = await dao.GetAll();
            return allBrands;
        }

        [HttpPost("seed")]
        public async Task<ActionResult> SeedBrands()
        {
            if (await _ctx.Brands!.AnyAsync())
            {
                return BadRequest("Brands already exist in the database");
            }

            var brands = new List<Brand>
            {
                new Brand { Name = "Nike" },
                new Brand { Name = "Adidas" },
                new Brand { Name = "Puma" },
                new Brand { Name = "Under Armour" },
                new Brand { Name = "Reebok" }
            };

            await _ctx.Brands!.AddRangeAsync(brands);
            await _ctx.SaveChangesAsync();

            return Ok("Brands seeded successfully");
        }
    }
}
