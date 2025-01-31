﻿using FaasNet.Runtime.AsyncAPI.v2.Models.Bindings;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FaasNet.Runtime.AsyncAPI.v2.Models
{
    public class ChannelItem 
    {
        /// <summary>
        /// An optional description of this channel item. CommonMark syntax can be used for rich text representation.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// The servers on which this channel is available, specified as an optional unordered list of names (string keys) of Server Objects defined in the Servers Object (a map).
        /// </summary>
        [JsonProperty("servers")]
        public IEnumerable<string> Servers { get; set; }
        /// <summary>
        /// A definition of the SUBSCRIBE operation, which defines the messages produced by the application and sent to the channel.
        /// </summary>
        [JsonProperty("subscribe")]
        public Operation Subscribe { get; set; }
        /// <summary>
        /// A definition of the PUBLISH operation, which defines consumed by the application from the channel.
        /// </summary>
        [JsonProperty("publish")]
        public Operation Publish { get; set; }
        /// <summary>
        /// A free-form map where the keys describe the name of the protocol and the values describe protocol-specific definitions for the channel.
        /// </summary>
        [JsonProperty("bindings")]
        public ChannelBindings Bindings { get; set; }
    }
}
