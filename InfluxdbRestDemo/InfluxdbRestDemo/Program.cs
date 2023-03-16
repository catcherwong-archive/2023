using System.Text.Json;

const string influxUrl = "http://localhost:8086";
const string orgId = "489b62f26bb28ff2";
const string bucket = "test-benchmarks";

var apiToken = "3QiJYkkol8UkwwK9AV_VIo4hXaD5a3ZwRzPaN6OBN_jIOLIXdVgPC-zRTWjsOGccToMPUGleiddJwF-0HZbz_Q==";

CheckAndCreateBucket(bucket, apiToken);
WriteData(bucket, apiToken);

static void WriteData(string bucket, string token)
{
    var data = GenLineProtocolData();
    var reqUrl = $"{influxUrl}/api/v2/write?orgID={orgId}&bucket={bucket}";
    using HttpClient client = new();
    StringContent content = new(data.ToString());
    HttpRequestMessage reqMsg = new(HttpMethod.Post, reqUrl);
    reqMsg.Content = content;
    reqMsg.Headers.TryAddWithoutValidation("Authorization", $"Token {token}");
    var resp = client.SendAsync(reqMsg).GetAwaiter().GetResult();
    Console.WriteLine(resp.StatusCode);
}

static string GenLineProtocolData()
{
    var data = new System.Text.StringBuilder();

    var dt = DateTime.UtcNow.AddDays(-2);
    var session = Guid.NewGuid().ToString("N");

    for (int i = 1; i < 100; i++)
    {
        data.Append($"application.benchmarks/cpu,session={session},scenario=hello,netSdkVersion=3.1.426,AspNetCoreVersion=3.1.32+3eeb12e,NetCoreAppVersion=3.1.32+f94bb2c,hw=Unspecified,env=Unspecified,os=Windows,arch=X64,proc=2 value={new Random().NextDouble() * 100} {GetTimestamp(dt.AddMinutes(-i))}\n");
    }

    return data.ToString();
}

static string GetTimestamp(DateTime datetime)
{
    var ts = datetime.Subtract(DateTime.UnixEpoch).Ticks * 100;
    return ts.ToString();
}

static void CheckAndCreateBucket(string bucket, string token)
{
    var reqUrl = $"{influxUrl}/api/v2/buckets?orgID={orgId}&name={bucket}";
    using (HttpClient client = new())
    {
        HttpRequestMessage httpReq = new(HttpMethod.Get, reqUrl);
        httpReq.Headers.TryAddWithoutValidation("Authorization", $"Token {token}");
        var resp = client.SendAsync(httpReq).GetAwaiter().GetResult();

        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Console.WriteLine("Not Found, should create");
            CreateBucket(bucket, token);
        }

        Console.WriteLine(resp.StatusCode);
    }

    static void CreateBucket(string bucket, string token)
    {
        var reqUrl = $"{influxUrl}/api/v2/buckets";
        using HttpClient client = new();
        var data = new { description = "", name = bucket, orgID = orgId };
        StringContent content = new(JsonSerializer.Serialize(data));
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        HttpRequestMessage httpReq = new(HttpMethod.Post, reqUrl);
        httpReq.Content = content;
        httpReq.Headers.TryAddWithoutValidation("Authorization", $"Token {token}");
        var resp = client.SendAsync(httpReq).GetAwaiter().GetResult();
        resp.EnsureSuccessStatusCode();
        Console.WriteLine(resp.StatusCode);
    }
}
