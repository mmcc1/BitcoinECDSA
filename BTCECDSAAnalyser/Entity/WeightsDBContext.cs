using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BTCECDSAAnalyser.Entity.Tables;

namespace BTCECDSAAnalyser.Entity
{
    class WeightsDBContext : DbContext
    {
        public DbSet<WeightStatistics> WeightStatistic { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=127.0.0.1;Database=xxx;User Id=xxx;Password=xxxx;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
