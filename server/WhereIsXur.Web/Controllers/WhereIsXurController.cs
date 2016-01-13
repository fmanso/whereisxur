using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using WhereIsXur.Web.Services;

namespace WhereIsXur.Web.Controllers
{
    public class WhereIsXurController : ApiController
    {
        // GET: WhereIsXur        
        [Route("api/whereisxur")]
        public async Task<HttpResponseMessage> Get()
        {
            var location = await SearchXur(DateTime.UtcNow);

            return new HttpResponseMessage()
            {
                Content = new StringContent(location, Encoding.ASCII)
            };
        }

        [Route("api/whereisxur/items")]
        public async Task<HttpResponseMessage> GetItems()
        {
            var data = await SearchItems(DateTime.UtcNow);

            return new HttpResponseMessage()
            {
                Content = new StringContent(data, Encoding.ASCII)
            };
        }

        private static async Task<string> SearchItems(DateTime today)
        {
            var whereIsXur = new WhereIsXurService();
            if (!whereIsXur.IsXurWorking(today))
            {
                return string.Empty;
            }
            else
            {
                var post = await whereIsXur.SearchPost(today);
                if (post == null)
                {
                    return string.Empty;
                }

                var exotics = whereIsXur.ParseExotics(post);

                if (string.IsNullOrEmpty(exotics))
                {
                    return string.Empty;
                }
                
                return exotics;
            }
        }

        private static async Task<string> SearchXur(DateTime today)
        {
            var whereIsXur = new WhereIsXurService();
            if (!whereIsXur.IsXurWorking(today))
            {
                return "Xur is out of office right now";
            }
            else
            {
                var post = await whereIsXur.SearchPost(today);
                if (post == null)
                {
                    return "Searching for Xur...\nCome back in a couple of minutes.";
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
