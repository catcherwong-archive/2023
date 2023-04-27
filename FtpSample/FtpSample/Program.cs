using FluentFTP;
using FluentFTP.Helpers;
using System.Text.Encodings.Web;
using System.Text.Json;

var host = "127.0.0.1";
var username = "ftp";
var passwd = "ftp1234567890";
var path = "/pics";

ListFiles(host, username, passwd, path);

var localBasePath = Path.Combine(AppContext.BaseDirectory, "pics");

UploadFile(host, username, passwd, Path.Combine(localBasePath, "test.png"), $"{path}/test.png");

DownloadFile(host, username, passwd, Path.Combine(localBasePath, "down", "a.png"), $"{path}/test.png");

Console.ReadKey();

static void ListFiles(string host, string username, string passwd, string path)
{
    using var con = new FtpClient(host, username, passwd);
    con.Connect();

    var list = con.GetListing(path);

    foreach (var item in list)
    {
        Console.WriteLine(JsonSerializer.Serialize(item, new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }));
    }
}

static void UploadFile(string host, string username, string passwd, string localPath, string remotePath)
{
    using var con = new FtpClient(host, username, passwd);
    con.Connect();
    
    var res = con.UploadFile(localPath, remotePath);

    if (res.IsSuccess())
    {
        Console.WriteLine("OK");
    }
    else
    {
        Console.WriteLine("Error");
    }
}

static void DownloadFile(string host, string username, string passwd, string localPath, string remotePath)
{
    using var con = new FtpClient(host, username, passwd);
    con.Connect();

    var res = con.DownloadFile(localPath, remotePath);

    if (res.IsSuccess())
    {
        Console.WriteLine("OK");
    }
    else
    {
        Console.WriteLine("Error");
    }
}
