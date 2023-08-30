using System;
using System.Threading.Tasks;

namespace Net31App
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new GrpcService.Greeter.GreeterClient(ClientLib.Utils.GetGrpcChannel("http://localhost:5089"));
            var res = await client.SayHelloAsync(new GrpcService.HelloRequest { Name = "World" });
            Console.WriteLine(res.Message);
            Console.WriteLine("Hello World!");
        }
    }
}
