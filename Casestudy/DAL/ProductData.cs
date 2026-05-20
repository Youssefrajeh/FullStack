using Casestudy.DAL.DomainClasses;
using System.Text.Json;

namespace Casestudy.DAL
{
    public class ProductData
    {
        private readonly AppDbContext _db;

        public ProductData(AppDbContext ctx)
        {
            _db = ctx;
        }

        public async Task<bool> LoadBrandsAndProductsFromWebToDb(string json)
        {
            bool brandsLoaded = false;
            bool productsLoaded = false;

            try
            {
                Console.WriteLine("Starting to deserialize JSON...");
                dynamic? jsonObjectArray = JsonSerializer.Deserialize<object>(json);
                Console.WriteLine("JSON deserialized successfully");

                Console.WriteLine("Starting to load brands...");
                brandsLoaded = await LoadBrands(jsonObjectArray);
                Console.WriteLine($"Brands loaded: {brandsLoaded}");

                if (brandsLoaded)
                {
                    Console.WriteLine("Starting to load products...");
                    productsLoaded = await LoadProducts(jsonObjectArray);
                    Console.WriteLine($"Products loaded: {productsLoaded}");
                }
                else
                {
                    Console.WriteLine("Skipping product load because brands failed to load");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION in LoadBrandsAndProductsFromWebToDb: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return brandsLoaded && productsLoaded;
        }

        private async Task<bool> LoadBrands(dynamic jsonArray)
        {
            try
            {
                Console.WriteLine("Clearing existing brands...");
                _db.Brands?.RemoveRange(_db.Brands);
                await _db.SaveChangesAsync();
                Console.WriteLine("Existing brands cleared");

                List<string> brandNames = new();
                Console.WriteLine("Extracting brand names from JSON...");
                foreach (JsonElement element in jsonArray.EnumerateArray())
                {
                    if (element.TryGetProperty("BRAND", out JsonElement brandJson))
                    {
                        string? brand = brandJson.GetString();
                        if (!string.IsNullOrWhiteSpace(brand))
                        {
                            brandNames.Add(brand);
                        }
                    }
                }
                Console.WriteLine($"Found {brandNames.Count} brand names");

                var distinctBrands = brandNames.Distinct();
                Console.WriteLine($"Found {distinctBrands.Count()} distinct brands");
                foreach (string name in distinctBrands)
                {
                    Brand b = new() { Name = name };
                    await _db.Brands!.AddAsync(b);
                }

                await _db.SaveChangesAsync();
                Console.WriteLine("Brands saved to database successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in LoadBrands: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private async Task<bool> LoadProducts(dynamic jsonArray)
        {
            try
            {
                Console.WriteLine("Clearing existing products...");
                _db.Products?.RemoveRange(_db.Products);
                await _db.SaveChangesAsync();
                Console.WriteLine("Existing products cleared");

                List<Brand> brands = _db.Brands!.ToList();
                Console.WriteLine($"Found {brands.Count} brands in database");

                int productsAdded = 0;
                int productsSkipped = 0;
                foreach (JsonElement element in jsonArray.EnumerateArray())
                {
                    try
                    {
                        string? id = element.TryGetProperty("ID", out var idJson) ? idJson.GetString() : null;
                        string? name = element.TryGetProperty("NAME", out var nameJson) ? nameJson.GetString() : null;
                        string? graphic = element.TryGetProperty("GRAPHIC", out var graphicJson) ? graphicJson.GetString() : null;
                        string? description = element.TryGetProperty("DESCRIPTION", out var descJson) ? descJson.GetString() : null;
                        string? brandName = element.TryGetProperty("BRAND", out var brandJson) ? brandJson.GetString() : null;

                        decimal cost = element.TryGetProperty("COST", out var costJson) && decimal.TryParse(costJson.GetString(), out var c) ? c : 0;
                        decimal msrp = element.TryGetProperty("MSRP", out var msrpJson) && decimal.TryParse(msrpJson.GetString(), out var m) ? m : 0;
                        int qoh = element.TryGetProperty("QOH", out var qohJson) && int.TryParse(qohJson.GetString(), out var q) ? q : 0;
                        int bo = element.TryGetProperty("BO", out var boJson) && int.TryParse(boJson.GetString(), out var b) ? b : 0;

                        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(brandName))
                        {
                            Console.WriteLine($"Skipping product with missing required fields. ID: {id}, Name: {name}, Brand: {brandName}");
                            productsSkipped++;
                            continue;
                        }

                        Brand? match = brands.FirstOrDefault(br => br.Name == brandName);
                        if (match != null)
                        {
                            Product p = new()
                            {
                                Id = id,
                                ProductName = name,
                                GraphicName = graphic,
                                Description = description,
                                CostPrice = cost,
                                MSRP = msrp,
                                QtyOnHand = qoh,
                                QtyOnBackOrder = bo,
                                BrandId = match.Id,
                                Brand = match
                            };

                            await _db.Products!.AddAsync(p);
                            productsAdded++;
                        }
                        else
                        {
                            Console.WriteLine($"⚠️ No brand match found for product '{name}' with brand '{brandName}'");
                            productsSkipped++;
                        }
                    }
                    catch (Exception innerEx)
                    {
                        Console.WriteLine($"⚠️ Error in product loop: {innerEx.Message}");
                        productsSkipped++;
                    }
                }

                await _db.SaveChangesAsync();
                Console.WriteLine($"Products saved to database successfully. Added: {productsAdded}, Skipped: {productsSkipped}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in LoadProducts: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}
