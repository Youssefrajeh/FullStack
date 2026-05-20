using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        private readonly IWebHostEnvironment _env;

        public DataController(AppDbContext context, IWebHostEnvironment env) // injected here
        {
            _ctx = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Index()
        {
            DataUtility util = new(_ctx);
            string payload;
            var json = await GetMenuItemJsonFromWebAsync();
            try
            {
                payload = (await util.LoadNutritionInfoFromWebToDb(json)) ? "tables loaded" : "problem loading tables";
            }
            catch (Exception ex)
            {
                payload = ex.Message;
            }
            return JsonSerializer.Serialize(payload);
        }

        private static async Task<string> GetMenuItemJsonFromWebAsync()
        {
            string url = "https://raw.githubusercontent.com/elauersen/info3067/master/mcdonalds.json";
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        [Route("loadbranches")]
        [HttpGet]
        public async Task<ActionResult<string>> LoadBranches()
        {
            string payload;
            BranchDAO dao = new(_ctx);
            bool branchesLoaded = await dao.LoadBranchesFromFile(_env.WebRootPath);
            try
            {
                payload = branchesLoaded ? "branches loaded successfully" : "problem loading branch data";
            }
            catch (Exception ex)
            {
                payload = ex.Message;
            }
            return JsonSerializer.Serialize(payload);
        }
    }
}