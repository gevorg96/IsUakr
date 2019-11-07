using System;
using System.Collections.Generic;
using System.Linq;
using IsUakr.DAL;

namespace IsUakr.Parcer
{
    public class DataParcer
    {
        private readonly string conn;

        public DataParcer(string connectionString)
        {
            conn = connectionString;
        }
        
        public void Run()
        {
            List<House> houses;
            var streets = new List<Street>();
            using (var db = new NpgDbContext(conn))
            {
                houses = db.Houses.ToList();
                streets = db.Streets.ToList();
                foreach (var house in houses)
                {
                    var parts = house.number.Split(new string[] {", д."}, StringSplitOptions.RemoveEmptyEntries);
                    string streetStr;
                    if (parts.Length == 1)
                    {
                        house.number = "д. " + parts[0].Replace(", Домодедово", "");
                        streetStr = "Домодедово";
                    }
                    else
                    {
                        house.number = "д. " + parts[1].Replace(", Домодедово", "");
                        streetStr = parts[0].Replace("г. Домодедово, ", "");
                    }
                   
                    Street street;
                    if (streets.Exists(p => p.name == streetStr))
                    {
                        street = streets.FirstOrDefault(p => p.name == streetStr);
                        street.Houses.Add(house);
                    }
                    else
                    {
                        street = new Street
                        {
                            name = streetStr
                        };
                        street.Houses.Add(house);
                        streets.Add(street);
                    }
                }

                db.SaveChanges();

            }
        }
        
        
    }
}