using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using IsUakr.DAL;
using Newtonsoft.Json;

namespace IsUakr.Parcer
{
    public class Parcer
    {
        private string conn;

        public Parcer(string conn)
        {
            this.conn = conn;
        }

        public async Task RunAsync()
        {
            var housesUrl = @"http://dom.mingkh.ru";

            var housesResponse = await LoadData(housesUrl + "/api/houses");
            var housesJObject = JsonConvert.DeserializeObject<DataGO>(housesResponse);
            var housesList = new List<House>();
            foreach (var house in housesJObject.rows)
            {
                var h = new House
                {
                    id = house.rownumber,
                    square = house.square != "Незаполнено" ? Convert.ToDouble(house.square.Replace(".", ",")) : 0,
                    fullAddress = house.address,
                    year = house.year != "Не заполнено" ? Convert.ToInt32(house.year) : 0,
                    manageStartDate = house.managestartdate != "Не заполнено"
                        ? Convert.ToDateTime(house.managestartdate)
                        : DateTime.MinValue,
                    number = house.url.Split('/').Last(),
                    floors = house.floors != "Не заполнено" ? Convert.ToInt32(house.floors) : 0
                };

                var result = await LoadPage(housesUrl + house.url) as string;
                HtmlDocument hap = new HtmlDocument();
                hap.LoadHtml(result);
                var dtnodes = hap.DocumentNode.SelectNodes("//dt");
                var ddnodes = hap.DocumentNode.SelectNodes("//dd");

                var idx = 0;
                for (int i = 0; i < dtnodes.Count; i++)
                {
                    if (dtnodes[i].InnerHtml == "Жилых помещений")
                    {
                        idx = i;
                        break;
                    }
                }

                int flats = 0;
                var isConverted = Int32.TryParse(ddnodes[idx].InnerHtml, out flats);
                if (isConverted)
                {
                    h.flatsCount = flats;
                    housesList.Add(h);
                }
            }

            using (var db = new NpgDbContext(conn))
            {
                foreach (var h in housesList)
                {
                    var (streetStr, houseStr) = GetStreetAndHouse(h.fullAddress);

                    var streetDal = db.Streets.FirstOrDefault(p => p.name == streetStr);

                    if (streetDal != null)
                    {
                        var house = new House
                        {
                            number = houseStr,
                            square = Convert.ToDouble(h.square),
                            year = Convert.ToInt32(h.year),
                            code = Convert.ToInt32(h.code),
                            floors = Convert.ToInt32(h.floors),
                            flatsCount = Convert.ToInt32(h.flatsCount),
                            manageStartDate = Convert.ToDateTime(h.manageStartDate),
                            Street = streetDal

                        };
                        db.Houses.Add(house);
                        db.SaveChanges();
                    }
                    else
                    {
                        var street = new Street
                        {
                            name = streetStr
                        };
                        db.Streets.Add(street);
                        db.SaveChanges();



                        var house = new House
                        {
                            number = houseStr,
                            square = Convert.ToDouble(h.square),
                            year = Convert.ToInt32(h.year),
                            code = Convert.ToInt32(h.code),
                            floors = Convert.ToInt32(h.floors),
                            flatsCount = Convert.ToInt32(h.flatsCount),
                            manageStartDate = Convert.ToDateTime(h.manageStartDate),
                            Street = db.Streets.FirstOrDefault(p => p.name == streetStr)

                        };
                        db.Houses.Add(house);
                        db.SaveChanges();
                    }
                }


                foreach (var house in db.Houses.ToList())
                {
                    var meterHub = new MeterHub
                    {
                        House = house,
                        code = Guid.NewGuid().ToString()
                    };
                    db.MeterHubs.Add(meterHub);

                    for (int i = 0; i < house.flatsCount; i++)
                    {
                        var flat = new Flat
                        {
                            Num = (i + 1).ToString(),
                            House = house
                        };
                        db.Flats.Add(flat);



                        var energyMeter = new Meter
                        {
                            code = Guid.NewGuid().ToString(),
                            type = "energy",
                            Flat = flat,
                            Hub = meterHub
                        };
                        var heat_water = new Meter
                        {
                            code = Guid.NewGuid().ToString(),
                            type = "heat_water",
                            Flat = flat,
                            Hub = meterHub
                        };
                        var cold_water = new Meter
                        {
                            code = Guid.NewGuid().ToString(),
                            type = "cold_water",
                            Flat = flat,
                            Hub = meterHub
                        };
                        db.Meters.Add(energyMeter);
                        db.Meters.Add(heat_water);
                        db.Meters.Add(cold_water);

                    }
                }

                db.SaveChanges();
            }
        }

        static async Task<string> LoadData(string url)
        {
            var result = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Post;
            var sb = new StringBuilder();
            sb.Append("current=1&rowCount=-1&searchPhrase=&region_url=moskovskaya-oblast&city_url=domodedovo");
            var data = Encoding.ASCII.GetBytes(sb.ToString());
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            
            var response = await request.GetResponseAsync();

            var resp = response as HttpWebResponse;

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var receiveStream = response.GetResponseStream();
                if (receiveStream != null)
                {
                    StreamReader readStream;
                    if (resp.CharacterSet == null)
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(resp.CharacterSet));
                    result = readStream.ReadToEnd();
                    readStream.Close();
                }
                response.Close();
            }

            return result;
        }


        static async Task<object> LoadPage(string url)
        {
            var result = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Get;
            var response = await request.GetResponseAsync();
            var resp = response as HttpWebResponse;

            if (resp.StatusCode == HttpStatusCode.OK)
            {
                var receiveStream = response.GetResponseStream();
                if (receiveStream != null)
                {
                    StreamReader readStream;
                    if (resp.CharacterSet == null)
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(resp.CharacterSet));
                    result = readStream.ReadToEnd();
                    readStream.Close();
                }
                response.Close();
            }

            return result;
        }


        private (string, string) GetStreetAndHouse(string address)
        {
            var parts = address.Split(new string[] {", д."}, StringSplitOptions.RemoveEmptyEntries);
            string streetStr;
            if (parts.Length == 1)
            {
                address = "д. " + parts[0].Replace(", Домодедово", "");
                streetStr = "Домодедово";
            }
            else
            {
                address = "д. " + parts[1].Replace(", Домодедово", "");
                streetStr = parts[0].Replace("г. Домодедово, ", "");
            }

            return (streetStr, address);

        }
    }
}