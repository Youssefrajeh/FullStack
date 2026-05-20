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
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public ProductController(AppDbContext context)
        {
            _ctx = context;
        }

        [HttpGet("brand/{brandId}")]
        public async Task<ActionResult<List<Product>>> GetProductsByBrand(int brandId)
        {
            var products = await _ctx.Products!
                .Where(p => p.BrandId == brandId)
                .ToListAsync();

            return products;
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<Product>>> GetProductsByCategory(int categoryId)
        {
            ProductDAO dao = new(_ctx);
            List<Product> productsForCategory = await dao.GetAllByBrand(categoryId);
            return productsForCategory;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            ProductDAO dao = new(_ctx);
            List<Product> products = await dao.GetAll();
            return products;
        }

        [HttpPost("checkstock")]
        public async Task<ActionResult<Dictionary<string, object>>> CheckProductStock([FromBody] string[] productIds)
        {
            try
            {
                var stockInfo = new Dictionary<string, object>();

                foreach (var productId in productIds)
                {
                    var product = await _ctx.Products!.FindAsync(productId);
                    if (product != null)
                    {
                        stockInfo[productId] = new
                        {
                            QtyOnHand = product.QtyOnHand,
                            QtyOnBackOrder = product.QtyOnBackOrder,
                            ProductName = product.ProductName
                        };
                    }
                    else
                    {
                        stockInfo[productId] = new
                        {
                            QtyOnHand = 0,
                            QtyOnBackOrder = 0,
                            ProductName = "Product not found"
                        };
                    }
                }

                return Ok(stockInfo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("seed")]
        public async Task<ActionResult> SeedProducts()
        {
            if (await _ctx.Products!.AnyAsync())
            {
                return BadRequest("Products already exist in the database");
            }

            var products = new List<Product>
            {
                new Product
                {
                    Id = "UF001",
                    BrandId = 1, // UrbanFurnish
                    ProductName = "Modern Sofa",
                    GraphicName = "sofa.jpg",
                    CostPrice = 599.99m,
                    MSRP = 899.99m,
                    QtyOnHand = 10,
                    QtyOnBackOrder = 0,
                    Description = "Contemporary 3-seater sofa with premium fabric upholstery"
                },
                new Product
                {
                    Id = "UF002",
                    BrandId = 1, // UrbanFurnish
                    ProductName = "Coffee Table",
                    GraphicName = "coffee-table.jpg",
                    CostPrice = 199.99m,
                    MSRP = 299.99m,
                    QtyOnHand = 15,
                    QtyOnBackOrder = 5,
                    Description = "Glass top coffee table with wooden frame"
                },
                new Product
                {
                    Id = "UF003",
                    BrandId = 1, // UrbanFurnish
                    ProductName = "Dining Set",
                    GraphicName = "dining-set.jpg",
                    CostPrice = 799.99m,
                    MSRP = 1199.99m,
                    QtyOnHand = 5,
                    QtyOnBackOrder = 2,
                    Description = "6-piece dining set with chairs and table"
                },
                new Product
                {
                    Id = "UF004",
                    BrandId = 1, // UrbanFurnish
                    ProductName = "Bookshelf",
                    GraphicName = "bookshelf.jpg",
                    CostPrice = 149.99m,
                    MSRP = 249.99m,
                    QtyOnHand = 8,
                    QtyOnBackOrder = 0,
                    Description = "Modern 5-shelf bookcase with adjustable shelves"
                }
            };

            await _ctx.Products!.AddRangeAsync(products);
            await _ctx.SaveChangesAsync();

            return Ok("Products seeded successfully");
        }
    }
}