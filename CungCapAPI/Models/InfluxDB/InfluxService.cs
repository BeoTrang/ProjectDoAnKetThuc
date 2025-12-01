using CungCapAPI.Models.Redis;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLibrary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;


namespace CungCapAPI.Services
{
    public class InfluxService
    {
        private readonly InfluxDBClient _client;
        private readonly string _org;
        private readonly string _bucket;
        private readonly IRedisService _Redis;

        public InfluxService(IConfiguration config, IRedisService Redis)
        {
            var url = config["InfluxDb:Url"];
            var token = config["InfluxDb:Token"];
            _org = config["InfluxDb:Org"];
            _bucket = config["InfluxDb:Bucket"];
            _Redis = Redis;

            _client = InfluxDBClientFactory.Create(url, token.ToCharArray());
        }

        public async Task WriteSensorAsync(string payload)
        {
            var data = JsonDocument.Parse(payload);
            var type = data.RootElement.GetProperty("type").GetString();
            var writeApi = _client.GetWriteApiAsync();

            switch (type)
            {
                case "AX01":
                    var ax01_new = JsonConvert.DeserializeObject<AX01<DHT22, Relay4, Name_AX01>>(payload);



                    //Ghi nhiệt độ, độ ẩm
                    var data_AX01 = PointData
                        .Measurement("sensor_data")
                        .Tag("device_id", ax01_new.id)
                        .Tag("type", ax01_new.type)
                        .Tag("data", "data")
                        .Field("tem", ax01_new.data.tem)
                        .Field("hum", ax01_new.data.hum)
                        .Timestamp(ax01_new.timestamp, WritePrecision.Ns);
                    await writeApi.WritePointAsync(data_AX01, _bucket, _org);

                    //Kiểm tra relay có thay đổi không, rồi ghi vào 
                    var key = $"device:{ax01_new.id}:data";
                    var redis_data = await _Redis.GetAsync(key);
                    if (redis_data.IsNullOrEmpty())
                    {
                        var relay_AX01 = PointData
                                .Measurement("sensor_data")
                                .Tag("device_id", ax01_new.id)
                                .Tag("type", ax01_new.type)
                                .Tag("data", "relay")
                                .Field("relay1", ax01_new.relays.relay1)
                                .Field("relay2", ax01_new.relays.relay2)
                                .Field("relay3", ax01_new.relays.relay3)
                                .Field("relay4", ax01_new.relays.relay4)
                                .Timestamp(ax01_new.timestamp, WritePrecision.Ns);
                        await writeApi.WritePointAsync(relay_AX01, _bucket, _org);
                    }
                    else
                    {
                        var ax01_redis = JsonConvert.DeserializeObject<AX01<DHT22, Relay4, Name_AX01>>(redis_data);
                        if (ax01_new.relays.relay1 != ax01_redis.relays.relay1 || ax01_new.relays.relay2 != ax01_redis.relays.relay2 || ax01_new.relays.relay3 != ax01_redis.relays.relay3 || ax01_new.relays.relay4 != ax01_redis.relays.relay4)
                        {
                            var relay_AX01 = PointData
                                .Measurement("sensor_data")
                                .Tag("device_id", ax01_new.id)
                                .Tag("type", ax01_new.type)
                                .Tag("data", "relay")
                                .Field("relay1", ax01_new.relays.relay1)
                                .Field("relay2", ax01_new.relays.relay2)
                                .Field("relay3", ax01_new.relays.relay3)
                                .Field("relay4", ax01_new.relays.relay4)
                                .Timestamp(ax01_new.timestamp, WritePrecision.Ns);
                            await writeApi.WritePointAsync(relay_AX01, _bucket, _org);
                        }
                    }

                        break;

                case "AX02":
                    var ax02 = JsonConvert.DeserializeObject<AX02<DHT22, Name_AX02>>(payload);
                    
                    var data_AX02 = PointData
                        .Measurement("sensor_data")
                        .Tag("device_id", ax02.id)
                        .Tag("type", ax02.type)
                        .Field("tem", ax02.data.tem)
                        .Field("hum", ax02.data.hum)
                        .Timestamp(ax02.timestamp, WritePrecision.Ns);
                    await writeApi.WritePointAsync(data_AX02, _bucket, _org);
                    break;

                default:

                    break;
            }
        }

        public async Task<List<object>> LichSuDuLieuThietBi(HistorySearch model)
        {
            if ((model.startUTC == null || model.endUTC == null) && model.pickTime == null)
            {
                return null;
            }

            string flux = "";
            string time = "";
            string fillterType = "";
            string aggregateWindow = "";


            //if (model.startUTC != null && model.endUTC != null)
            //{
            //    time = $"start: {model.startUTC:o}, stop: {model.endUTC:o}";
            //}
            //else 
            if (model.pickTime != null)
            {
                switch (model.pickTime)
                {
                    case "-1h":
                        time = "range(start: -1h)";
                        aggregateWindow = "|> aggregateWindow(every: 5m, fn: mean, createEmpty: false)";
                        break;
                    case "-3h":
                        time = "range(start: -3h)";
                        aggregateWindow = "|> aggregateWindow(every: 10m, fn: mean, createEmpty: false)";
                        break;
                    case "-6h":
                        time = "range(start: -6h)";
                        aggregateWindow = "|> aggregateWindow(every: 15m, fn: mean, createEmpty: false)";
                        break;
                    case "-12h":
                        time = "range(start: -12h)";
                        aggregateWindow = "|> aggregateWindow(every: 30m, fn: mean, createEmpty: false)";
                        break;
                    case "-24h":
                        time = "range(start: -24h)";
                        aggregateWindow = "|> aggregateWindow(every: 30m, fn: mean, createEmpty: false)";
                        break;
                    case "-2d":
                        time = "range(start: -2d)";
                        aggregateWindow = "|> aggregateWindow(every: 30m, fn: mean, createEmpty: false)";
                        break;
                    case "-7d":
                        time = "range(start: -7d)";
                        aggregateWindow = "|> aggregateWindow(every: 1h, fn: mean, createEmpty: false)";
                        break;
                    case "-15d":
                        time = "range(start: -15d)";
                        aggregateWindow = "|> aggregateWindow(every: 3h, fn: mean, createEmpty: false)";
                        break;
                    case "-30d":
                        time = "range(start: -30d)";
                        aggregateWindow = "|> aggregateWindow(every: 6h, fn: mean, createEmpty: false)";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }

            if (model.type != null)
            {
                switch (model.typePick)
                {
                    case ("TemHum"):
                        fillterType = @"r[""_field""] == ""hum"" or r[""_field""] == ""tem""";
                        break;
                    case ("Relay"):
                        fillterType = @"r[""_field""] == ""relay1"" or r[""_field""] == ""relay2"" or r[""_field""] == ""relay3"" or r[""_field""] == ""relay4""";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }

            switch(model.typePick)
            {
                case ("TemHum"):
                    flux = $@"
                        from(bucket: ""{_bucket}"")
                          |> {time}
                          |> filter(fn: (r) => r[""_measurement""] == ""sensor_data"")
                          |> filter(fn: (r) => r[""type""] == ""{model.type}"")
                          |> filter(fn: (r) => r[""device_id""] == ""{model.deviceId}"")
                          |> filter(fn: (r) => {fillterType})
                          {aggregateWindow}
                          |> pivot(rowKey: [""_time""], columnKey: [""_field""], valueColumn: ""_value"")
                          |> yield(name: ""mean"")";
                    break;
                case ("Relay"):
                    flux = $@"
                        from(bucket: ""{_bucket}"")
                          |> {time}
                          |> filter(fn: (r) => r._measurement == ""sensor_data"")
                          |> filter(fn: (r) => r.device_id == ""{model.deviceId}"" and r.type == ""{model.type}"")
                          |> filter(fn: (r) => {fillterType})
                          |> difference(nonNegative: false)
                          |> filter(fn: (r) => r._value != 0.0)
                          |> keep(columns: [""_time"", ""_field"", ""_value""])
                          |> yield(name: ""relayChanges"")";
                    break;
                default:
                    return null;
            }

            

            var queryApi = _client.GetQueryApi();
            var tables = await queryApi.QueryAsync(flux, _org);
            var list = new List<dynamic>();
            var sapxepList = new List<dynamic>();
            switch (model.typePick)
            {
                case ("TemHum"):
                    foreach (var table in tables)
                    {
                        foreach (var record in table.Records)
                        {
                            list.Add(new
                            {
                                time = record.GetTime()?.ToDateTimeUtc(),
                                hum = record.Values.ContainsKey("hum") ? record.Values["hum"] : null,
                                tem = record.Values.ContainsKey("tem") ? record.Values["tem"] : null
                            });
                        }
                    }
                    break;
                case ("Relay"):
                    foreach (var table in tables)
                    {
                        foreach (var record in table.Records)
                        {
                            var value = Convert.ToDouble(record.GetValue());
                            var state = value > 0 ? "BẬT" : "TẮT";
                            list.Add(new
                            {
                                time = record.GetTime()?.ToDateTimeUtc(),
                                relay = record.GetField(),
                                state = state
                            });
                        }
                    }
                    sapxepList = list.OrderByDescending(x => x.time).ToList();
                    break;
                default:
                    return null;
            }
            
            if (sapxepList.Any())
            {
                return sapxepList;
            }
            else
            {
                return list;
            }

            //string flux = $@"
            //    from(bucket: ""{_bucket}"")
            //      |> range(start: {startUtc.ToString("o", CultureInfo.InvariantCulture)}, stop: {stopUtc.ToString("o", CultureInfo.InvariantCulture)})
            //      |> filter(fn: (r) => r._measurement == ""sensor_data"")
            //      |> filter(fn: (r) => {fieldFilter})
            //      |> filter(fn: (r) => r.device_id == ""{deviceId}"")
            //      |> filter(fn: (r) => r.type == ""{type}"")
            //      |> aggregateWindow(every: {windowPeriod}, fn: mean, createEmpty: false)
            //      |> yield(name: ""mean"")";
        }
    }
}
