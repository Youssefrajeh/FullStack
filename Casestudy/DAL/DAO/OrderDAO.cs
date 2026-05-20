using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Casestudy.DAL.DAO
{
    public class OrderDAO
    {
        private readonly AppDbContext _db;

        public OrderDAO(AppDbContext context)
        {
            _db = context;
        }

        public async Task<OrderResult> AddOrder(int customerId, OrderSelectionDTO[] selections)
        {
            int orderId = -1;
            var backOrderedItems = new List<BackOrderedItem>();

            using (var _trans = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    Order order = new Order
                    {
                        OrderDate = DateTime.Now,
                        CustomerId = customerId,
                        OrderAmount = selections.Sum(item => item.Qty * item.MSRP)
                    };

                    await _db.Orders.AddAsync(order);
                    await _db.SaveChangesAsync();
                    orderId = order.Id;

                    // process line items and update inventory
                    foreach (var item in selections)
                    {
                        Product? product = await _db.Products.FindAsync(item.ProductId);
                        if (product != null)
                        {
                            OrderLineItem lineItem = new OrderLineItem
                            {
                                OrderId = orderId,
                                ProductId = item.ProductId,
                                SellingPrice = item.MSRP,
                                QtyOrdered = item.Qty
                            };

                            // Scenario 1: Enough stock
                            if (item.Qty <= product.QtyOnHand)
                            {
                                product.QtyOnHand -= item.Qty;
                                lineItem.QtySold = item.Qty;
                                lineItem.QtyBackOrdered = 0;
                            }
                            // Scenario 2: Not enough stock
                            else
                            {
                                int availableStock = product.QtyOnHand;
                                int backOrderQty = item.Qty - availableStock;
                                lineItem.QtySold = availableStock;
                                lineItem.QtyBackOrdered = backOrderQty;
                                product.QtyOnBackOrder += backOrderQty;

                                // Track backordered items
                                backOrderedItems.Add(new BackOrderedItem
                                {
                                    ProductId = item.ProductId,
                                    ProductName = product.ProductName!,
                                    QtyRequested = item.Qty,
                                    QtyAvailable = availableStock,
                                    QtyBackOrdered = backOrderQty
                                });

                                product.QtyOnHand = 0;
                            }

                            await _db.OrderLineItems.AddAsync(lineItem);
                            _db.Products.Update(product);
                        }
                    }

                    await _db.SaveChangesAsync();
                    await _trans.CommitAsync();
                }
                catch (Exception ex)
                {
                    await _trans.RollbackAsync();
                    throw new Exception("Error processing order: " + ex.Message);
                }
            }

            return new OrderResult
            {
                OrderId = orderId,
                BackOrderedItems = backOrderedItems
            };
        }

        public async Task<List<Order>> GetAll(int customerId)
        {
            return await _db.Orders!.Where(order => order.CustomerId == customerId).ToListAsync<Order>();
        }

        public async Task<List<OrderDetailsHelper>> GetOrderDetails(int orderId, int customerId)
        {
            List<OrderDetailsHelper> allDetails = new();

            // LINQ way of doing INNER JOINS
            var results = from o in _db.Orders!
                          join oli in _db.OrderLineItems! on o.Id equals oli.OrderId
                          join p in _db.Products! on oli.ProductId equals p.Id
                          where (o.CustomerId == customerId && o.Id == orderId)
                          select new OrderDetailsHelper
                          {
                              OrderId = o.Id,
                              CustomerId = o.CustomerId,
                              OrderDate = o.OrderDate.ToString("yyyy/MM/dd - hh:mm tt"),
                              OrderAmount = o.OrderAmount,
                              ProductId = oli.ProductId,
                              ProductName = p.ProductName!,
                              QtyOrdered = oli.QtyOrdered,
                              QtySold = oli.QtySold,
                              QtyBackOrdered = oli.QtyBackOrdered,
                              SellingPrice = oli.SellingPrice
                          };

            allDetails = await results.ToListAsync();
            return allDetails;
        }

        public async Task<bool> ClearOrderHistory(int customerId)
        {
            try
            {
                // Get all orders for the customer
                var orders = await _db.Orders!.Where(o => o.CustomerId == customerId).ToListAsync();

                if (!orders.Any())
                {
                    return false; // No orders to delete
                }

                // Get all order line items for these orders
                var orderIds = orders.Select(o => o.Id).ToList();
                var orderLineItems = await _db.OrderLineItems!.Where(oli => orderIds.Contains(oli.OrderId)).ToListAsync();

                // Remove order line items first (due to foreign key constraints)
                _db.OrderLineItems!.RemoveRange(orderLineItems);

                // Remove orders
                _db.Orders!.RemoveRange(orders);

                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public class OrderSelectionDTO
    {
        public string ProductId { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal MSRP { get; set; }
    }

    public class OrderResult
    {
        public int OrderId { get; set; }
        public List<BackOrderedItem> BackOrderedItems { get; set; } = new List<BackOrderedItem>();
    }

    public class BackOrderedItem
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QtyRequested { get; set; }
        public int QtyAvailable { get; set; }
        public int QtyBackOrdered { get; set; }
    }
}