using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public sealed class NpgDbContext: DbContext
    {
        private string connectionString;
        public DbSet<House> Houses { get; set; }
        public DbSet<Street> Streets { get; set; }
         
        public NpgDbContext(string conn)
        {
            connectionString = conn;
            //Database.EnsureCreated();
        }
         
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}