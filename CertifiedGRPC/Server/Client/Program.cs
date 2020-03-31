using System;
using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Certify;

namespace Client
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            await CallCertificateInfo(includeClientCertificate: true);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task CallCertificateInfo(bool includeClientCertificate)
        {
            try
            {
                Console.WriteLine($"Setting up HttpClient. Client has certificate: {includeClientCertificate}");

                var channel = GrpcChannel.ForAddress("https://localhost:5002", new GrpcChannelOptions
                {
                    HttpClient = CreateHttpClient(includeClientCertificate)

                });
                var client = new Certifier.CertifierClient(channel);

                Console.WriteLine("Sending gRPC call...");
                var certificateInfo = await client.GetCertificateInfoAsync(new Empty());

                Console.WriteLine($"Server received client certificate: {certificateInfo.HasCertificate}");
                if (certificateInfo.HasCertificate)
                {
                    Console.WriteLine($"Client certificate name: {certificateInfo.Name}");
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"gRPC error from calling service: {ex.Status.Detail}");
            }
            catch
            {
                Console.WriteLine($"Unexpected error calling service.");
                throw;
            }
        }

        private static HttpClient CreateHttpClient(bool includeClientCertificate)
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.SslProtocols = SslProtocols.Tls12;
            if (includeClientCertificate)
            {
                // Load client certificate
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var certPath = Path.Combine(basePath, "Certs", "client.pfx");
                var clientCertificate = new X509Certificate2(certPath, "1111");
                Console.WriteLine(clientCertificate.Thumbprint);
                handler.ClientCertificates.Add(clientCertificate);
            }

            var client = new HttpClient(handler);
            client.DefaultRequestVersion = new Version(2, 0);
            return client;
        }

    }
}