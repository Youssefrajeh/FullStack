using Casestudy.DAL;
using Casestudy.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace Casestudy.DAL.DAO
{
    public class BrandsDAO
    {
        private readonly AppDbContext _db;
        public BrandsDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public async Task<List<Brand>> GetAll()
        {
            return await _db.Brands!.ToListAsync();
        }
    }
}