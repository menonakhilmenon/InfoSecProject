
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(kestrelOptions =>
                    {
                        kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            httpsOptions.AllowAnyClientCertificate();
                            httpsOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });

        //private static X509Certificate2 GetCertificate()
        //{
        //    var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
        //    var certPath = Path.Combine(basePath, "Certs", "client.pfx");
        //    var clientCertificate = new X509Certificate2(certPath, "1111");

        //    Console.WriteLine(clientCertificate.Thumbprint);
        //    return clientCertificate;
        //}
    }
}
