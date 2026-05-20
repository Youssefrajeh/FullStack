using System.Text.Json;
using Casestudy.DAL.DomainClasses;

namespace Casestudy.DAL
{
    public class DataUtility
    {
        private readonly AppDbContext _db;

        public DataUtility(AppDbContext context)
        {
            _db = context;
        }

        public async Task<bool> LoadNutritionInfoFromWebToDb(string stringJson)
        {
            try
            {
                // Clear out the old data
                if (_db.Products != null && _db.Brands != null)
                {
                    _db.Products.RemoveRange(_db.Products);
                    _db.Brands.RemoveRange(_db.Brands);
                    await _db.SaveChangesAsync();
                }

                // Parse the JSON string into objects
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // You'll need to define these classes based on your JSON structure
                var menuItems = JsonSerializer.Deserialize<List<MenuItem>>(stringJson, jsonOptions);

                if (menuItems != null)
                {
                    // Process each menu item
                    foreach (var item in menuItems)
                    {
                        // Add brand if it doesn't exist
                        var brand = new Brand
                        {
                            Name = item.BrandName
                        };
                        await _db.Brands!.AddAsync(brand);
                        await _db.SaveChangesAsync();

                        // Add product
                        var product = new Product
                        {
                            BrandId = brand.Id,
                            ProductName = item.ProductName,
                            GraphicName = item.GraphicName,
                            CostPrice = item.CostPrice,
                            MSRP = item.MSRP,
                            QtyOnHand = item.QtyOnHand,
                            QtyOnBackOrder = item.QtyOnBackOrder,
                            Description = item.Description
                        };
                        await _db.Products!.AddAsync(product);
                        await _db.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }

    // Helper class to deserialize JSON
    public class MenuItem
    {
        public string BrandName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string GraphicName { get; set; } = string.Empty;
        public decimal CostPrice { get; set; }
        public decimal MSRP { get; set; }
        public int QtyOnHand { get; set; }
        public int QtyOnBackOrder { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}