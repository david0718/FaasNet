﻿using FaasNet.EventMesh.Runtime.Messages;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public interface IMessageHandler
    {
        Commands Command { get; }
        Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken);
    }
}
