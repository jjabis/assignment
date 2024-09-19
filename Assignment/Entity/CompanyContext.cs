using Microsoft.EntityFrameworkCore;

namespace Assignment.Entity
{
    public class CompanyContext : DbContext
    {
        public virtual DbSet<Company> Company { get; set; } 
        public CompanyContext(DbContextOptions<CompanyContext> options) : base(options) { }
    }
}
