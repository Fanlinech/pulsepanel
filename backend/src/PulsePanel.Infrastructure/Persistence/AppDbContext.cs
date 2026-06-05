using Microsoft.EntityFrameworkCore;
using PulsePanel.Core.Entities;

namespace PulsePanel.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Server> Servers { get; set; }
    }
}