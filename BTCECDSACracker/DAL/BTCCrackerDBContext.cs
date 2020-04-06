using Microsoft.EntityFrameworkCore;
using BTCECDSACracker.DAL.Tables;

namespace BTCECDSACracker.DAL
{
    public class BTCCrackerDBContext : DbContext
    {
        public DbSet<WeightLog> WeightLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=127.0.0.1;Database=XXX;User Id=XXX;Password=XXX;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
