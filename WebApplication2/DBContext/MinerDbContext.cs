using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.DBContext
{
    public class MinerDbContext : DbContext
    {
        public MinerDbContext(DbContextOptions<MinerDbContext> options) : base(options)
        {

        }
        public DbSet<Schools> Schools { get; set; }
        public DbSet<Students> Students { get; set; }
        public DbSet<Ratings> Ratings { get; set; }

    }
}
