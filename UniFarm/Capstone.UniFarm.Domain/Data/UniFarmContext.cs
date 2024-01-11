using Capstone.UniFarm.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.UniFarm.Domain.Data
{
    public partial class UniFarmContext : DbContext
    {
        public UniFarmContext(DbContextOptions<UniFarmContext> options) : base(options)
        {
        }
        public virtual DbSet<Category> Categories { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
