using Microsoft.EntityFrameworkCore;
using BTCECDSACracker.DAL.Tables;

namespace BTCECDSACracker.DAL
{
    public class BTCCrackerDBContext : DbContext
    {
        public DbSet<WeightLog> WeightLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=127.0.0.1;Database=xxx;User Id=xxx;Password=xxx;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
