using Blazorise.Snackbar;
using IsUakr.DAL;
using IsUakr.Entities.Messages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IsUakr.HighLoad.Ui.Helpers
{
    public class Generator
    {
        private readonly string _url;
        private readonly NpgDbContext _db;

        private readonly object _lock = new object();

        public Generator(string url, NpgDbContext db)
        {
            _url = url;
            _db = db;
        }

        public List<HubMessage> Generate(IEnumerable<Flat> flats)
        {
            var groups = flats.GroupBy(x => x.House);

            var rnd = new Random();
            var hubMessages = new List<HubMessage>();


            Parallel.ForEach(groups, group =>
            {
                MeterHub hub = null;
                IEnumerable<Meter> meters = null;
                IEnumerable<Flat> flts = null;

                lock (_lock)
                {
                    hub = _db.MeterHubs.FirstOrDefault(p => p.House.id == group.Key.id);
                    meters = _db.Meters.Include(p => p.Flat).Where(p => p.Hub.id == hub.id).ToList();
                    flts = meters.Select(p => p.Flat).Distinct().OrderBy(p => p.Num).ToList();
                }

                var hubMessage = new HubMessage()
                {
                    HubId = hub.id,
                    Messages = new List<MeterMessage>()
                };

                foreach (var flat in group)
                {
                    var f = flts.FirstOrDefault(x => x.Id == flat.Id);
                    foreach (var meter in f.Meters)
                    {
                        var meterMsg = new MeterMessage()
                        {
                            Id = meter.id,
                            Address = Int32.Parse(f.Num),
                            Env = 1,
                            Maker = "lora",
                            SerialNumber = meter.id,
                            SetupIdentity = 1,
                            Body = new StateMessage
                            {
                                MeasureUnit = GetMeterType(meter.type),
                                Message = "OK",
                                State = 1,
                                MeterDt = DateTime.Now,
                                Volume = float.Parse((200 + rnd.NextDouble() * rnd.Next(0, 800)).ToString())
                            }
                        };
                        hubMessage.Messages.Add(meterMsg);
                    }
                }
                hubMessages.Add(hubMessage);
            });
            return hubMessages;
        }

        public bool SendToServer(IEnumerable<HubMessage> hubMessages)
        {
            try
            {
                Parallel.ForEach(hubMessages, mess =>
                {
                    var http = new HttpClient();
                    var content = new StringContent(mess.ToString(), Encoding.UTF8, "application/json");
                    http.PostAsync("http://localhost:50826/api/Message", content);
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private short GetMeterType(string type)
        {
            return type switch
            {
                "energy" => 1,
                "heat_water" => 2,
                "cold_water" => 3,
                _ => 0
            };
        }
    }
}
