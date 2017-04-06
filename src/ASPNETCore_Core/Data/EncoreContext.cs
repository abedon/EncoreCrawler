using ASPNETCore_Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore_Core.Data
{
    public class EncoreContext : DbContext
    {
        public EncoreContext(DbContextOptions<EncoreContext> options) : base(options)
        {
        }

        public DbSet<Encore> Encores { get; set; }
        public DbSet<EncoreMatch> EncoreMatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Encore>().ToTable("Encore");
            modelBuilder.Entity<EncoreMatch>().ToTable("EncoreMatch");
        }
    }
}
