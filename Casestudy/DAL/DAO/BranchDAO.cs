using Casestudy.DAL.DomainClasses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Casestudy.DAL.DAO
{
    public class BranchDAO
    {
        private readonly AppDbContext _db;
        public BranchDAO(AppDbContext context)
        {
            _db = context;
        }

        public async Task<bool> LoadBranchesFromFile(string path)
        {
            bool addWorked = false;
            try
            {
                // clear out the old rows
                if (_db.Branches != null)
                {
                    _db.Branches.RemoveRange(_db.Branches);
                    await _db.SaveChangesAsync();
                }

                var csv = new List<string[]>();
                var csvFile = path + "\\branchLocations.txt";
                Console.WriteLine($"Loading branches from file: {csvFile}");

                if (!System.IO.File.Exists(csvFile))
                {
                    Console.WriteLine("Branch locations file not found!");
                    return false;
                }

                var lines = await System.IO.File.ReadAllLinesAsync(csvFile);
                Console.WriteLine($"Found {lines.Length} lines in file");
                foreach (string line in lines)
                    csv.Add(line.Split(',')); // populate branch with csv

                foreach (string[] rawdata in csv)
                {
                    try
                    {
                        Console.WriteLine($"Processing line: {string.Join(",", rawdata)}");
                        Branch aBranch = new()
                        {
                            Longitude = Convert.ToDouble(rawdata[0]),
                            Latitude = Convert.ToDouble(rawdata[1]),
                            Street = rawdata[2],
                            City = rawdata[3],
                            Region = rawdata[4]
                        };
                        await _db.Branches!.AddAsync(aBranch);
                        await _db.SaveChangesAsync();
                        Console.WriteLine($"Successfully added branch: {aBranch.Street}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing branch: {ex.Message}");
                        throw; // Re-throw to be caught by outer try-catch
                    }
                }
                addWorked = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return addWorked;
        }

        public async Task<List<Branch>?> GetThreeClosestBranches(float? lat, float? lon)
        {
            List<Branch>? branchDetails = null;
            try
            {
                var latParam = new SqlParameter("@lat", lat);
                var lonParam = new SqlParameter("@lon", lon);
                var query = _db.Branches?.FromSqlRaw("dbo.pGetThreeClosestBranches @lat, @lon", latParam, lonParam);
                branchDetails = await query!.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return branchDetails;
        }
    }
}