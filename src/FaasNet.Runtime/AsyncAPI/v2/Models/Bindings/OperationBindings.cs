﻿using FaasNet.Runtime.AsyncAPI.v2.Models.Bindings.Amqp;
using FaasNet.Runtime.JSchemas;
using Newtonsoft.Json;
using NJsonSchema.References;

namespace FaasNet.Runtime.AsyncAPI.v2.Models.Bindings
{
    [JsonConverter(typeof(ReferenceConverter))]
    public class OperationBindings : JsonReferenceBase<OperationBindings>, IJsonReference
    {
        [JsonProperty("amqp")]
        public AmqpOperationBinding Amqp { get; set; }

        public IJsonReference ActualObject { get; }

        public object PossibleRoot { get; }
    }
}
