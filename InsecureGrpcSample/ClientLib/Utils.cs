using Grpc.Core;
using Grpc.Net.Client;
using System;

namespace ClientLib
{
    public class Utils
    {
        public static GrpcChannel GetGrpcChannel(string address)
        {
            bool isSecure = false;
            if (address.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            { 
                isSecure = true;
            }

            var options = new GrpcChannelOptions();
            if (!isSecure)
            {
#if NET5_0_OR_GREATER
                options.Credentials = ChannelCredentials.Insecure;
#else
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
#endif
            }

            return GrpcChannel.ForAddress(address, options);
        }
    }
}