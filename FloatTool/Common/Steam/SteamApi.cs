using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace FloatTool.Steam
{
    /*public static class SteamApi
    {
        public readonly static HttpClient httpClient = new();

        public struct RequestDefinition
        {
            public string ApiInterface;
            public string ApiMethod;
            public string ApiVersion;
            public dynamic Data;
            public string? AccessToken;
        }

        public static async Task<dynamic> SendRequest(RequestDefinition request)
        {
            string url = $"https://api.steampowered.com/{request.ApiInterface}/{request.ApiMethod}/v{request.ApiVersion}/";
            var content = new FormUrlEncodedContent(request.Data);
            var result = await httpClient.PostAsync(url, content);
            
            return null;
        }
    }*/
}