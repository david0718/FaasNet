﻿using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Core.Resources;
using FaasNet.EventMesh.Core.Vpn.Queries.Results;
using FaasNet.EventMesh.Runtime.Stores;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Vpn.Queries.Handlers
{
    public class GetAllApplicationDomainQueryHandler : IRequestHandler<GetAllApplicationDomainQuery, ApplicationDomainResult>
    {
        private readonly IVpnStore _vpnStore;

        public GetAllApplicationDomainQueryHandler(IVpnStore vpnStore)
        {
            _vpnStore = vpnStore;
        }

        public async Task<ApplicationDomainResult> Handle(GetAllApplicationDomainQuery request, CancellationToken cancellationToken)
        {
            var vpn = await _vpnStore.Get(request.Vpn, cancellationToken);
            if (vpn == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_VPN, string.Format(Global.UnknownVpn, request.Vpn));
            }

            var applicationDomain = vpn.ApplicationDomains.FirstOrDefault(a => a.Id == request.ApplicationDomainId);
            if (applicationDomain == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_APPLICATIONDOMAIN, string.Format(Global.UnknownApplicationDomain, request.ApplicationDomainId));
            }

            return ApplicationDomainResult.Build(applicationDomain);
        }
    }
}
