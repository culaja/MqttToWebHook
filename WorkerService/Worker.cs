using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using WorkerService.Mqtt;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                void OnMessageReceived(MqttApplicationMessageReceivedEventArgs obj)
                {
                    var receivedPayload = Encoding.ASCII.GetString(obj.ApplicationMessage.Payload);
                    _logger.LogInformation($"[PayloadReceived] {receivedPayload}");

                    var response = WebHookSender.SendTo(_configuration.WebHookUrl(), receivedPayload);
                    _logger.Log(response.IsSuccessStatusCode ? LogLevel.Debug : LogLevel.Error, $"[WebHookResult] {response.StatusCode}");
                }

                using var mqttReceiver = MqttReceiver.CreateSubscriptionFor(
                    _configuration.MqttConfiguration(),
                    OnMessageReceived);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);    
                }
            }
        }

        
    }
}