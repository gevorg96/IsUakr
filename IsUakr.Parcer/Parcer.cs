using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Microsoft.EntityFrameworkCore;
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

        public void Run()
        {
            /*var streets = @"https://m.vsedomarossii.ru/api/data/streets/2190";
            var houses = @"https://m.vsedomarossii.ru/api/data/houses/2190";

            var streetsTask = Task.Run(() => LoadData(streets));
            var housesTask = Task.Run(() => LoadData(houses));

            Task.WhenAll(streetsTask, housesTask);

            var streetsData = JsonConvert.DeserializeObject<List<StreetJson>>(streetsTask.Result.ToString());
            var housesData = JsonConvert.DeserializeObject<List<HouseJson>>(housesTask.Result.ToString());

            housesData = housesData.Where(p => !string.IsNullOrEmpty(p.num)).ToList();
            
            using (var db = new NpgDbContext(conn))
            {
                var streets = db.Streets.Include(u => u.Houses).ToList();
                var houses = db.Houses.Include(u => u.Street).ToList();
                foreach (var street in streetsData)
                {
                    var str = new Street
                    {
                        id = street.id,
                        name = street.name,
                        socr = street.socr
                    };
                    db.Streets.Add(str);
                }

                db.SaveChanges();

                foreach (var house in housesData)
                {
                    var h = new House
                    {
                        id = house.id,
                        num = house.num,
                        typeId = house.type,
                        Street = db.Streets.Find(house.streetId)
                    };
                    db.Houses.Add(h);
                }

                db.SaveChanges();
            }*/
        }


        static async Task<object> LoadData(string url)
        {
            var result = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Post;
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
    }
}