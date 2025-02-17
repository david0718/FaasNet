﻿namespace FaasNet.EventMesh.Runtime.Messages
{
    public class HelloRequest : Package
    {
        public UserAgent UserAgent { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            UserAgent.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            UserAgent = UserAgent.Deserialize(context);
        }
    }
}
