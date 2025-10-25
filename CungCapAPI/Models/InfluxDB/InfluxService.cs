using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;
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
                switch (model.type, model.typePick)
                {
                    case ("AX01", "TemHum"):
                        fillterType = @"r[""_field""] == ""hum"" or r[""_field""] == ""tem""";
                        break;
                    case ("AX01", "Relay"):
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

            switch(model.type, model.typePick)
            {
                case ("AX01", "TemHum"):
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
                case ("AX01", "Relay"):
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
            switch (model.type, model.typePick)
            {
                case ("AX01", "TemHum"):
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
                case ("AX01", "Relay"):
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
