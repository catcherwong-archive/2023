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
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) 
                => ValidateCertificate(sender, cert, chain, sslPolicyErrors)
    };

    try
    {
        using CancellationTokenSource cts = new(3000);
        using HttpClient client = new(clientHandler);
        await client.GetAsync($"https://{domain}", cts.Token);

        Console.WriteLine($"{domain} is OK {nameof(HttpClientSSLCheck)}");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("canceled");
    }
    catch (Exception e)
    {
        if (e.InnerException is CertPolicyException)
        {
            Console.WriteLine($"{domain} | ex = {e.InnerException.Message}");
        }
        else if (e.InnerException is NeedRenewException)
        {
            Console.WriteLine($"{domain} | need to renew !!");
        }
        else
        {
            Console.WriteLine($"{domain} | ex = {e.Message}");
        }
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
        await ssl.AuthenticateAsClientAsync(domain);

        Console.WriteLine($"{domain} is OK {nameof(TcpClientSSLCheck)}");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("canceled");
    }
    catch (Exception e)
    {
        HandleException(domain, e);
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
        await ssl.AuthenticateAsClientAsync(domain);

        Console.WriteLine($"{domain} is OK {nameof(SocketSSLCheck)}");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("canceled");
    }
    catch (Exception e)
    {
        HandleException(domain, e);
    }
}

static bool ValidateCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
{
    if (certificate == null) return true;

    var expirationDate = DateTime.Parse(certificate.GetExpirationDateString(), CultureInfo.InvariantCulture);
    if (expirationDate - DateTime.Today < TimeSpan.FromDays(30))
    {
        throw new NeedRenewException("It's time to renew the certificate!");
    }
    if (sslPolicyErrors == SslPolicyErrors.None)
    {
        return true;
    }
    else
    {
        throw new CertPolicyException($"Cert policy errors: {sslPolicyErrors.ToString()}");
    }
}

static void HandleException(string domain, Exception e)
{
    if (e is CertPolicyException)
    {
        Console.WriteLine($"{domain} | ex = {e.Message}");
    }
    else if (e is NeedRenewException)
    {
        Console.WriteLine($"{domain} | need to renew !!");
    }
    else
    {
        Console.WriteLine($"{domain} | ex = {e.Message}");
    }
}

public class NeedRenewException : Exception
{
    public NeedRenewException(string msg) : base(msg)
    {
    }
}

public class CertPolicyException : Exception
{
    public CertPolicyException(string msg) : base(msg)
    {
    }
}