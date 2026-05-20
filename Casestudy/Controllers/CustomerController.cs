using Casestudy.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Casestudy.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _ctx;

        public CustomerController(AppDbContext ctx)
        {
            _ctx = ctx;
        }


    }
}
