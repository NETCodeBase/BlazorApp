using Microsoft.EntityFrameworkCore;

namespace BlazorAzureSignalR.Data
{
    public class AppDbContext : DbContext
    {
        string _connectionString = "Server=signalrnotifications2024.database.windows.net;Initial Catalog=pocemployees;Persist Security Info=False;User ID=sudheer;Password=betageri@1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public DbSet<Employee> Employee { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }

}
