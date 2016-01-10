using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using WhereIsXur.Web.Services;

namespace WhereIsXur.Web.Controllers
{
    public class WhereIsXurController : ApiController
    {
        // GET: WhereIsXur
        public async Task<HttpResponseMessage> Get()
        {
            var location = await SearchXur(DateTime.UtcNow);

            return new HttpResponseMessage()
            {
                Content = new StringContent(location, Encoding.ASCII)
            };
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
    }
}
