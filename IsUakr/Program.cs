using System;
using System.Linq;
using DAL;

namespace IsUakr
{
    class Program
    {
        static void Main(string[] args)
        {
            
            
            using (NpgDbContext db = new NpgDbContext(builder.ConnectionString))
            {

                var users = db.Houses.ToList();
                
                Console.WriteLine("Users list:");
                foreach (House u in users)
                {
                    Console.WriteLine($"{u.Id} -> {u.Name}");
                }
            }
            Console.Read();
        }
        
    }
}