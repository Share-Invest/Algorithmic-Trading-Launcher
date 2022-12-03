using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ShareInvest.Infrastructure.Http;
using ShareInvest.Models;

using System.Collections.Generic;

namespace ShareInvest.Services;

class Update : CoreRestClient
{
    internal async IAsyncEnumerable<FileVersionInfo> GetAsyncEnumerable(string appName)
    {
        var res = await GetAsync(nameof(FileVersionInfo),
                                 JToken.FromObject(new
                                 {
                                     app = appName
                                 }));
        if (res is string json)
        {
            var enumerable = JsonConvert.DeserializeObject<FileVersionInfo[]>(json);

            if (enumerable != null)
            {
                foreach (var obj in enumerable)

                    yield return obj;
            }
        }
    }
    internal Update(string url) : base(url)
    {

    }
}