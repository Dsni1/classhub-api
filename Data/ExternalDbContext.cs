using Microsoft.EntityFrameworkCore;
using ClassHub.Models;

namespace ClassHub.Data
{
    public class ExternalDbContext : DbContext
    {
        public ExternalDbContext(DbContextOptions<ExternalDbContext> options)
            : base(options)
        {
        }

        public DbSet<Test> Tests { get; set; }
    }


}
