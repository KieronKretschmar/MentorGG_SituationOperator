using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SituationDatabase
{
    public class SituationContext : DbContext
    {
        public SituationContext()
        {

        }

        public SituationContext(DbContextOptions<SituationContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
