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
                configuration["Mqtt:ClientId"],
                configuration["Mqtt:SubscribeToTopic"]);

        public static string WebHookUrl(this IConfiguration configuration) => configuration["WebHookUrl"];

    }
}