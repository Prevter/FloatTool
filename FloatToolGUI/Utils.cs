using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FloatToolGUI
{
    class Utils
    {
        public static string CheckUpdates()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                using (var response = client.GetAsync("https://api.github.com/repos/nemeshio/FloatTool-GUI/releases/latest").Result)
                {
                    var json = response.Content.ReadAsStringAsync().Result;

                    dynamic release = JsonConvert.DeserializeObject(json);
                    return release["tag_name"];
                }
            }
        }
    }
}
