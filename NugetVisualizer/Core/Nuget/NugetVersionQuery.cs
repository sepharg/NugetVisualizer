using System;

namespace NugetVisualizer.Core.Nuget
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Newtonsoft.Json.Linq;

    public class NugetVersionQuery
    {
        public const string NOVERSIONFOUND = "no version found";

        public async Task<string> GetLatestVersion(string packageName)
        {
            // https://docs.microsoft.com/en-us/nuget/api/search-query-service-resource
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api-v2v3search-0.nuget.org");
                var versions = await client.GetAsync($"query?q={packageName}&prerelease=false");
                versions.EnsureSuccessStatusCode();
                var versionsContent = await versions.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(versionsContent);
                if (jObject["data"]?.First == null) return NOVERSIONFOUND;
                var version = (string)jObject["data"][0]["version"];
                return version;
            }
        }
    }
}
