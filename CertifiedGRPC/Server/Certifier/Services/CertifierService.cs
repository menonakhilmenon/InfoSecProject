using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Certify;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;

namespace Server
{

    public class CertifierService : Certifier.CertifierBase
    {
        [Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]
        public override Task<CertificateInfoResponse> GetCertificateInfo(Empty request, ServerCallContext context)
        {
            var httpContext = context.GetHttpContext();
            var clientCertificate = httpContext.Connection.ClientCertificate;
            Console.WriteLine(clientCertificate);
            var name = string.Join(',', context.AuthContext.PeerIdentity.Select(i => i.Value));
            var certificateInfo = new CertificateInfoResponse
            {
                HasCertificate = context.AuthContext.IsPeerAuthenticated,
                Name = name
            };

            return Task.FromResult(certificateInfo);
        }
    }
}
