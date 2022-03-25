﻿using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Runtime.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class ApplicationDomain
    {
        public ApplicationDomain()
        {
            MessageDefinitions = new List<MessageDefinition>();
            Applications = new List<Application>();
        }

        public string Id { get; set; }
        public string Vpn { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<MessageDefinition> MessageDefinitions { get; set; }
        public ICollection<Application> Applications { get; set; }

        public static ApplicationDomain Create(string vpn, string name, string description, string rootTopic)
        {
            var result = new ApplicationDomain
            {
                Id = Guid.NewGuid().ToString(),
                Vpn = vpn,
                Name = name,
                Description = description,
                RootTopic = rootTopic
            };
            return result;
        }

        public void AddMessageDefinition(MessageDefinition def)
        {
            var messageDef = MessageDefinitions.FirstOrDefault(m => m.Name == def.Name);
            if (messageDef != null)
            {
                throw new DomainException(ErrorCodes.MESSAGEDEF_ALREADY_EXISTS, Global.MessageDefAlreadyExists);
            }

            MessageDefinitions.Add(def);
        }
    }
}
