﻿using CloudNative.CloudEvents;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.AMQP
{
    public class AMQPPublisher : IMessagePublisher
    {
        private readonly AMQPOptions _options;
        private readonly IBrokerConfigurationStore _brokerConfigurationStore;

        public AMQPPublisher(
            IOptions<AMQPOptions> options,
            IBrokerConfigurationStore brokerConfigurationStore)
        {
            _options = options.Value;
            _brokerConfigurationStore = brokerConfigurationStore;
        }

        public string BrokerName
        {
            get
            {
                return _options.BrokerName;
            }
        }

        public Task Publish(CloudEvent cloudEvent, string topicName, Client client)
        {
            var options = GetOptions();
            var connectionFactory = new ConnectionFactory();
            options.ConnectionFactory(connectionFactory);
            using (var connection = connectionFactory.CreateConnection())
            {
                var channel = connection.CreateModel();
                var props = channel.CreateBasicProperties();
                cloudEvent.EnrichBasicProperties(props);
                channel.BasicPublish(exchange: options.TopicName,
                    routingKey: topicName,
                    basicProperties: props,
                    body: cloudEvent.SerializeBody());
            }

            return Task.CompletedTask;
        }

        private AMQPOptions GetOptions()
        {
            return _brokerConfigurationStore.Get(_options.BrokerName).ToAMQPOptions();
        }
    }
}
