using System.Data.Entity;

namespace Planet_CaseStudy.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<Task> Tasks { get; set; }
    }
}