using System.Globalization;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

await HttpClientSSLCheck("github.com");
await TcpClientSSLCheck("github.com");
await SocketSSLCheck("github.com");

static async Task HttpClientSSLCheck(string domain)
{
    HttpClientHandler clientHandler = new()
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => ValidateCertificate(sender, cert, chain, sslPolicyErrors)
    };

    try
    {
        using CancellationTokenSource cts = new(3000);
        using HttpClient client = new(clientHandler);
        await client.GetAsync($"https://{domain}", cts.Token);

        Console.WriteLine($"DONE {nameof(HttpClientSSLCheck)}");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("canceled");
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}

static async Task TcpClientSSLCheck(string domain)
{
    try
    {
        using TcpClient tcpClient = new();
        using CancellationTokenSource cts = new(3000);
        await tcpClient.ConnectAsync(domain, 443, cts.Token);

        using var stream = tcpClient.GetStream();
        using SslStream ssl = new(stream, false, ValidateCertificate);
        ssl.AuthenticateAsClient(domain);

        Console.WriteLine($"DONE {nameof(TcpClientSSLCheck)}");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("canceled");
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}

static async Task SocketSSLCheck(string domain)
{
    try
    {
        using Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        using CancellationTokenSource cts = new(3000);
        await socket.ConnectAsync(domain, 443, cts.Token);

        using SslStream ssl = new (new NetworkStream(socket, false), false, ValidateCertificate);
        ssl.AuthenticateAsClient(domain);

        Console.WriteLine($"DONE {nameof(SocketSSLCheck)}");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("canceled");
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}


static bool ValidateCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
{
    if (certificate == null) return true;

    var expirationDate = DateTime.Parse(certificate.GetExpirationDateString(), CultureInfo.InvariantCulture);
    if (expirationDate - DateTime.Today < TimeSpan.FromDays(30))
    {
        throw new Exception("It's time to renew the certificate!");
    }
    if (sslPolicyErrors == SslPolicyErrors.None)
    {
        return true;
    }
    else
    {
        throw new Exception($"Cert policy errors: {sslPolicyErrors.ToString()}");
    }
}
