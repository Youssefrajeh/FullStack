using Casestudy.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace Casestudy.DAL.DAO
{
    public class CustomerDAO
    {
        private readonly AppDbContext _context;

        public CustomerDAO(AppDbContext ctx)
        {
            _context = ctx;
        }

        public async Task<Customer?> GetByEmail(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer> Register(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
    }
}
