using CungCapAPI.Hubs;
using CungCapAPI.Services;
using InfluxDB.Client.Configurations;
using Microsoft.AspNetCore.SignalR;
using ModelLibrary;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using System.Text;

namespace CungCapAPI.MQTT
{
    public class MqttService
    {
        private readonly IHubContext<DeviceHub> _hubContext;
        private readonly IMqttClient _mqttClient;
        private readonly IConfiguration _config;
        private MqttClientOptions _options;
        private readonly InfluxService _influx;

        public MqttService(IConfiguration config, InfluxService influx, IHubContext<DeviceHub> hubContext)
        {
            _hubContext = hubContext;
            _config = config;
            _influx = influx;
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            // Khi kết nối thành công
            _mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("✅ MQTT connected!");

                var topic = _config["Mqtt:Topic"];
                if (!string.IsNullOrEmpty(topic))
                {
                    var subscribeOptions = new MqttClientSubscribeOptions
                    {
                        TopicFilters = { new MqttTopicFilter { Topic = topic } }
                    };
                    await _mqttClient.SubscribeAsync(subscribeOptions);
                    Console.WriteLine($"📡 Subscribed to topic: {topic}");
                }
            };

            // Khi mất kết nối
            _mqttClient.DisconnectedAsync += async e =>
            {
                Console.WriteLine("❌ MQTT disconnected! Trying to reconnect in 5s...");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    if (_options != null)
                    {
                        await _mqttClient.ConnectAsync(_options);
                        Console.WriteLine("🔄 Reconnected to MQTT broker!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Reconnect failed: {ex.Message}");
                }
            };

            // Khi nhận dữ liệu từ topic
            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                Console.WriteLine($"📩 Received on {topic}: {payload}");
                var data = JsonConvert.DeserializeObject<AX01<DHT22, Relay4>>(payload);
                string deviceId = "DeviceId_" + data.id.ToString();
                _hubContext.Clients.Group(deviceId).SendAsync("DeviceData", payload);
                _influx.WriteSensorAsync(payload);

                

                await Task.CompletedTask;
            };
        }

        public async Task ConnectAsync()
        {
            _options = new MqttClientOptionsBuilder()
                .WithClientId(_config["Mqtt:Name"])
                .WithTcpServer(_config["Mqtt:Host"], int.Parse(_config["Mqtt:Port"]))
                .WithCredentials(_config["Mqtt:Username"], _config["Mqtt:Password"])
                .WithCleanSession()
                .Build();

            await _mqttClient.ConnectAsync(_options);
        }

        public async Task PublishAsync(string topic, string message)
        {
            var appMsg = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _mqttClient.PublishAsync(appMsg);
            Console.WriteLine($"📤 Published to {topic}: {message}");
        }
    }
}
