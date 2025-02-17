﻿using FaasNet.EventMesh.Runtime.Acl;
using FaasNet.EventMesh.Runtime.Exceptions;
using FaasNet.EventMesh.Runtime.Messages;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Handlers
{
    public class HelloMessageHandler : IMessageHandler
    {
        private readonly IClientStore _clientSessionStore;
        private readonly IACLService _aclService;

        public HelloMessageHandler(
            IClientStore clientSessionStore,
            IACLService aclService)
        {
            _clientSessionStore = clientSessionStore;
            _aclService = aclService;
        }

        public Commands Command => Commands.HELLO_REQUEST;

        public async Task<Package> Run(Package package, IPEndPoint sender, CancellationToken cancellationToken)
        {
            var helloRequest = package as HelloRequest;
            if (!await _aclService.Check(helloRequest.UserAgent, PossibleActions.AUTHENTICATE, cancellationToken))
            {
                throw new RuntimeException(helloRequest.Header.Command, helloRequest.Header.Seq, Errors.NOT_AUTHORIZED);
            }

            var sessionId = TryCreateSession(helloRequest, sender);
            return PackageResponseBuilder.Hello(package.Header.Seq, sessionId);
        }

        private string TryCreateSession(HelloRequest request, IPEndPoint sender)
        {
            bool isUpdated = true;
            var client = _clientSessionStore.Get(request.UserAgent.ClientId);
            if (client == null)
            {
                client = Client.Create(request.UserAgent.ClientId, request.UserAgent.Urn);
                isUpdated = false;
            }

            var sessionId = client.AddSession(sender,
                request.UserAgent.Environment, 
                request.UserAgent.Pid, 
                request.UserAgent.Purpose, 
                request.UserAgent.BufferCloudEvents, 
                request.UserAgent.IsServer);
            if (isUpdated)
            {
                _clientSessionStore.Update(client);
            }
            else
            {
                _clientSessionStore.Add(client);
            }

            return sessionId;
        }
    }
}
