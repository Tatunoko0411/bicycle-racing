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
        public DbSet<Battle> Battles { get; set; }
        public DbSet<BattleLog> Battle_Logs { get; set; }
        public DbSet<Friend> Friends { get; set; }

        public DbSet<Mail> Mails { get; set; }
#if DEBUG
        readonly string connectionString =
            "server=localhost;database=bicycle_racing;user=jobi;password=jobi;";
#else
readonly string connectionString = "server=db-ge0202400.mysql.database.azure.com;port=3306;database=realtime_game241202;user=student;password=Yoshidajobi2024;SslMode=Required;";

#endif
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString,new MySqlServerVersion(new Version(8, 0)));

        }
    }
}
