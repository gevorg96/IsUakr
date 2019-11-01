using System;
using System.Linq;
using DAL;

namespace IsUakr
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new PostgreSqlConnectionStringBuilder(
                "d1adtvm6bavqp6",
                "ec2-174-129-253-62.compute-1.amazonaws.com",
                "8600d78661ce0ad3417bf056d64a807a5fbf6abc37091f05a99b53bb3dc25a13",
                "uqyvgpqzzrgslm",
                5432, 
                true, 
                true, 
                SslMode.Require);
            
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