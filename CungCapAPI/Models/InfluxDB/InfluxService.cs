using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using ModelLibrary;


namespace CungCapAPI.Services
{
    public class InfluxService
    {
        private readonly InfluxDBClient _client;
        private readonly string _org;
        private readonly string _bucket;

        public InfluxService(IConfiguration config)
        {
            var url = config["InfluxDb:Url"];
            var token = config["InfluxDb:Token"];
            _org = config["InfluxDb:Org"];
            _bucket = config["InfluxDb:Bucket"];

            _client = InfluxDBClientFactory.Create(url, token.ToCharArray());
        }

        public async Task WriteSensorAsync(string payload)
        {
            var data = JsonDocument.Parse(payload);
            var type = data.RootElement.GetProperty("type").GetString();

            switch (type)
            {
                case "AX01":
                    var ax01 = JsonConvert.DeserializeObject<AX01<DHT22, Relay4, Name_AX01>>(payload);
                    var writeApi = _client.GetWriteApiAsync();
                    var point = PointData
                        .Measurement("sensor_data")
                        .Tag("device_id", ax01.id)
                        .Tag("type", ax01.type)
                        .Field("tem", ax01.data.tem)
                        .Field("hum", ax01.data.hum)
                        .Field("relay1", ax01.relays.relay1)
                        .Field("relay2", ax01.relays.relay2)
                        .Field("relay3", ax01.relays.relay3)
                        .Field("relay4", ax01.relays.relay4)
                        .Timestamp(ax01.timestamp, WritePrecision.Ns);
                    await writeApi.WritePointAsync(point, _bucket, _org);

                    break;

                case "AX02":

                    break;
                default:

                    break;
            }
            //var writeApi = _client.GetWriteApiAsync();
            //var point = PointData
            //    .Measurement("sensor_data")
            //    .Tag("device_id", deviceId)
            //    .Tag("type", type)
            //    .Field("temperature", temp)
            //    .Field("humidity", hum)
            //    .Timestamp(timestamp, WritePrecision.Ns);

            //foreach (var relay in relays)
            //{
            //    point = point.Field(relay.Key, relay.Value);
            //}

            //await writeApi.WritePointAsync(point, _bucket, _org);
        }
    }
}
