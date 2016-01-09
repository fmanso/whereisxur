using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using WhereIsXur.Web.Services;

namespace WhereIsXur.Web.Controllers
{
    [Route("api/[controller]")]
    public class SearchXurController : Controller
    {
        [HttpGet]
        public async Task<string> Get()
        {
            var today = DateTime.UtcNow;
            return await SearchXur(today);
        }

        private static async Task<string> SearchXur(DateTime today)
        {
            var whereIsXur = new WhereIsXurService();
            if (!whereIsXur.IsXurWorking(today))
            {
                return "Xur is not working right now";
            }
            else
            {
                var post = await whereIsXur.SearchPost(today);
                if (post == null)
                {
                    return "Still searching for Xur. Come back in a couple of minutes.";
                }

                var location = whereIsXur.ParseLocation(post);
                if (string.IsNullOrEmpty(location))
                {
                    return "Something gone wrong with the search :(";
                }

                return location;
            }
        }

        [HttpGet("{day}/{month}/{year}")]
        public async Task<string> GetSpecificDay(int day, int month, int year)
        {
            var date = new DateTime(year, month, day);
            return await SearchXur(date);
        }
    }
}
