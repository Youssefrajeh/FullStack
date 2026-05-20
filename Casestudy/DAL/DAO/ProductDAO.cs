using Casestudy.DAL;
using Casestudy.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace Casestudy.DAL.DAO
{
    public class ProductDAO
    {
        private readonly AppDbContext _db;
        public ProductDAO(AppDbContext ctx)
        {
            _db = ctx;
        }

        public async Task<List<Product>> GetAllByBrand(int brandId)
        {
            return await _db.Products!.Where(item => item.BrandId == brandId).ToListAsync();
        }

        public async Task<List<Product>> GetAll()
        {
            return await _db.Products!.ToListAsync();

        }


    }
}