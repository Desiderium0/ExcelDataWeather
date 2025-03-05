using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Test.Models;

namespace Test.DataBas_MSSQL.Context
{
    public class DataBaseContext(DbContextOptions<DataBaseContext> option) : DbContext(option)
    {
        public DbSet<WeatherData> WeatherData => Set<WeatherData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
