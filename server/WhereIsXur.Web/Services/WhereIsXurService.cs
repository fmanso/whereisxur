using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WhereIsXur.Web.Services
{
    /// <summary>
    /// This service has utility methods to tell if Xur is working or not. 
    /// Search on reddit the Xur's Megathread
    /// And parse its location from the reddit
    /// </summary>
    public class WhereIsXurService
    {
        /// <summary>
        /// Given the the body of the reddit Xur's Megathread, returns the Xur's location.
        /// </summary>
        /// <param name="body">Body of the reddit Xur's Megathread</param>        
        public string ParseLocation(string body)
        {
            var regExp = new Regex(@"Location:\*\*\n\n(?<location>.*?)\n\n");
            var match = regExp.Match(body);
            return match.Groups["location"].Value;
        }

        /// <summary>
        /// Tells if Xur is working. Xur is working from UTC FRIDAY 9.00 AM TO UTC SUNDAY 9.00 AM
        /// </summary>
        /// <param name="dateTime">The time to tell if Xur is working</param>
        /// <returns>True if he is working, False otherwise</returns>
        public bool IsXurWorking(DateTime dateTime)
        {
            var utcTime = dateTime.ToUniversalTime();
            if (utcTime.DayOfWeek == DayOfWeek.Friday && utcTime.Hour >= 9)
            {
                return true;
            }

            if (utcTime.DayOfWeek == DayOfWeek.Saturday)
            {
                return true;
            }

            if (utcTime.DayOfWeek == DayOfWeek.Sunday && utcTime.Hour < 9)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Search the reddit Xur's Megathread for the given date
        /// Return null if it is not found or if in the date, xur is not working
        /// </summary>
        /// <param name="date">The date</param>
        /// <returns>Body of the reddit Xur's Megathread. Null if it is not found.</returns>
        public async Task<string> SearchPost(DateTime date)
        {
            if (!IsXurWorking(date)) return null;

            var utcTime = date.ToUniversalTime();
            var offsetDays = 0;

            if (utcTime.DayOfWeek == DayOfWeek.Saturday) offsetDays = 1;
            if (utcTime.DayOfWeek == DayOfWeek.Sunday) offsetDays = 2;

            var friday = utcTime.Subtract(new TimeSpan(offsetDays, 0, 0, 0));

            var searchDate = $"{friday.Year:0000}-{friday.Month:00}-{friday.Day:00}";
            var searchUrl = $"https://www.reddit.com/r/DestinyTheGame/search.json?q=+Xur+Megathread+%5B{searchDate}%5D&restrict_sr=on&sort=new&t=all";

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(new Uri(searchUrl));
            var body = await response.Content.ReadAsStringAsync();

            var json = (JObject)JsonConvert.DeserializeObject(body);

            var children = (JArray)json["data"]["children"];

            if (children.Count == 0) return null;

            foreach (var child in children)
            {
                if ((string)child["data"]["title"] == $"Xur Megathread [{searchDate}]")
                {
                    return (string)child["data"]["selftext"];
                }
            }

            return null;
        }
    }
}