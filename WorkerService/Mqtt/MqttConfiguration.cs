namespace WorkerService.Mqtt
{
    public sealed class MqttConfiguration
    {
        public string HostName { get; }

        public ushort Port { get; }

        public string UserName { get; }

        public string Password { get; }

        public string ClientId { get; }

        public string SubscribeToTopic { get; }

        public MqttConfiguration(
            string hostName,
            ushort port,
            string userName,
            string password,
            string clientId, string subscribeToTopic)
        {
            HostName = hostName;
            Port = port;
            UserName = userName;
            Password = password;
            ClientId = clientId;
            SubscribeToTopic = subscribeToTopic;
        }
    }
}