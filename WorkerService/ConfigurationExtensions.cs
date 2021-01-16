using Microsoft.Extensions.Configuration;
using WorkerService.Mqtt;

namespace WorkerService
{
    public static class ConfigurationExtensions
    {
        public static MqttConfiguration MqttConfiguration(this IConfiguration configuration) =>
            new MqttConfiguration(
                configuration["Mqtt:HostName"],
                ushort.Parse(configuration["Mqtt:Port"]),
                configuration["Mqtt:UserName"],
                configuration["Mqtt:Password"],
                bool.Parse(configuration["Mqtt:ShouldUseTls"]),
                configuration["Mqtt:CertificatePfxBase64"],
                configuration["Mqtt:CertificatePfxPassword"],
                bool.Parse(configuration["Mqtt:ShouldValidateBrokerCertificate"]),
                configuration["Mqtt:ClientId"],
                configuration["Mqtt:SubscribeToTopic"]);

        public static string WebHookUrl(this IConfiguration configuration) => configuration["WebHookUrl"];

    }
}