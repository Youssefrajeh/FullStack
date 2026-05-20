using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly OrderDAO _orderDAO;

        public OrderController(AppDbContext context)
        {
            _db = context;
            _orderDAO = new OrderDAO(context);
        }

        [HttpPost("addorder/{customerId}")]
        public async Task<ActionResult<object>> AddOrder(int customerId, [FromBody] OrderSelectionDTO[] selections)
        {
            try
            {
                var result = await _orderDAO.AddOrder(customerId, selections);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("{email}")]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> List(string email)
        {
            List<Order> orders;
            CustomerDAO cDao = new(_db);
            Customer? orderOwner = await cDao.GetByEmail(email);
            OrderDAO oDao = new(_db);
            orders = await oDao.GetAll(orderOwner!.Id);
            return orders;
        }

        [Route("{orderid}/{email}")]
        [HttpGet]
        public async Task<ActionResult<List<OrderDetailsHelper>>> GetOrderDetails(int orderid, string email)
        {
            CustomerDAO cDao = new(_db);
            Customer? orderOwner = await cDao.GetByEmail(email);
            OrderDAO oDao = new(_db);
            return await oDao.GetOrderDetails(orderid, orderOwner!.Id);
        }

        [Route("clear/{email}")]
        [HttpDelete]
        public async Task<ActionResult<object>> ClearOrderHistory(string email)
        {
            try
            {
                CustomerDAO cDao = new(_db);
                Customer? orderOwner = await cDao.GetByEmail(email);
                OrderDAO oDao = new(_db);
                bool success = await oDao.ClearOrderHistory(orderOwner!.Id);

                if (success)
                {
                    return Ok(new { message = "Order history cleared successfully" });
                }
                else
                {
                    return NotFound(new { message = "No orders found to clear" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}