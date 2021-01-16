using System;
using System.Security.Cryptography.X509Certificates;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;

namespace WorkerService.Mqtt
{
    public sealed class MqttConfiguration
    {
        public string HostName { get; }
        public ushort Port { get; }
        public string UserName { get; }
        public string Password { get; }
        public bool ShouldUseTls { get; }
        public string CertificatePfxBase64 { get; }
        public string CertificatePfxPassword { get; }
        public bool ShouldValidateBrokerCertificate { get; }
        public string ClientId { get; }
        public string SubscribeToTopic { get; }

        public MqttConfiguration(
            string hostName,
            ushort port,
            string userName,
            string password,
            bool shouldUseTls,
            string certificatePfxBase64,
            string certificatePfxPassword,
            bool shouldValidateBrokerCertificate,
            string clientId, 
            string subscribeToTopic)
        {
            HostName = hostName;
            Port = port;
            UserName = userName;
            Password = password;
            ShouldUseTls = shouldUseTls;
            CertificatePfxBase64 = certificatePfxBase64;
            CertificatePfxPassword = certificatePfxPassword;
            ShouldValidateBrokerCertificate = shouldValidateBrokerCertificate;
            ClientId = clientId;
            SubscribeToTopic = subscribeToTopic;
        }

        public ManagedMqttClientOptions ToClientOptions() =>
            new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(ToMqttClientOptions())
                .Build();

        private IMqttClientOptions ToMqttClientOptions()
        {
            var builder = new MqttClientOptionsBuilder();
            builder.WithClientId(ClientId)
                .WithCleanSession(false)
                .WithTcpServer(HostName, Port)
                .WithCredentials(UserName, Password);
            
            if (ShouldUseTls)
            {
                builder
                    .WithTls(o =>
                    {
                        o.UseTls = true;
                        o.Certificates = new[]
                        {
                            new X509Certificate2(Convert.FromBase64String(CertificatePfxBase64), CertificatePfxPassword)
                        };
                        o.AllowUntrustedCertificates = !ShouldValidateBrokerCertificate;
                    });
            }

            return builder.Build();
        }
        
    }
}