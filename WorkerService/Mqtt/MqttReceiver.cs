using System;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;

namespace WorkerService.Mqtt
{
    public sealed class MqttReceiver : IDisposable
    {
        private readonly IManagedMqttClient _client;
        
        public MqttReceiver(IManagedMqttClient client)
        {
            _client = client;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public static IDisposable CreateSubscriptionFor(
            MqttConfiguration configuration,
            Action<MqttApplicationMessageReceivedEventArgs> onMessageReceived)
        {
            var client = new MqttFactory().CreateManagedMqttClient();
            var connectionOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(configuration.ClientId)
                    .WithCleanSession(false)
                    .WithTcpServer(configuration.HostName, configuration.Port)
                    .WithCredentials(configuration.UserName, configuration.Password)
                    .Build())
                .Build();
            
            void OnMessageReceived(MqttApplicationMessageReceivedEventArgs m) => onMessageReceived(m);

            client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            client.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);
            client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnMessageReceived);
            
            client.StartAsync(connectionOptions).Wait();
            
            client.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic(configuration.SubscribeToTopic)
                .WithExactlyOnceQoS()
                .Build()).Wait();

            return new MqttReceiver(client);
        }

        private static void OnConnectingFailed(ManagedProcessFailedEventArgs obj)
        {
            Console.WriteLine(obj.Exception.Message);
        }

        private static void OnDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            Console.WriteLine(obj.Exception.Message);
        }

        private static void OnConnected(MqttClientConnectedEventArgs obj)
        {
            Console.WriteLine("Connected");
            Console.WriteLine(obj.AuthenticateResult.ReasonString);
        }
    }
}