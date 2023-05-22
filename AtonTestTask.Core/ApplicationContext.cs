using AtonTestTask.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace AtonTestTask.Core
{
    public class ApplicationContext: DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { 
            Database.Migrate();
        }
    }
}
