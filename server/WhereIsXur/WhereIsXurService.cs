using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WhereIsXur
{
    public class WhereIsXurService
    {       
        public string ParseLocation(string body)
        {
            var regExp = new Regex(@"Location:\*\*\n\n(?<location>.*?)\n\n");
            var match = regExp.Match(body);
            return match.Groups["location"].Value;
        }

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

        public async Task<string> SearchPost(DateTime today)
        {
            if (!IsXurWorking(today)) return null;

            var utcTime = today.ToUniversalTime();
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

            var children = (JArray) json["data"]["children"];

            if (children.Count == 0) return null;

            foreach (var child in children)
            {
                if ((string)child["data"]["title"] == $"Xur Megathread [{searchDate}]")
                {
                    return (string) child["data"]["selftext"];
                }
            }

            return null;
        }
    }
}
