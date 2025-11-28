using Microsoft.EntityFrameworkCore;
using bicycle_racing.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using bicycle_racing.Server.Models.Entities;


namespace bicycle_racing.Server.Models.Contexts
{
    public class GameDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        readonly string connectionString =
            "server=localhost;database=bicycle_racing;user=jobi;password=jobi;";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString,new MySqlServerVersion(new Version(8, 0)));

        }
    }
}
