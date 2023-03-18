using Microsoft.EntityFrameworkCore;


namespace TheProjector.Data.Persistence;

public class TheProjectorDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }

    public DbSet<Person> People { get; set; }

    public TheProjectorDbContext(DbContextOptions<TheProjectorDbContext> options)
    : base(options)
    {
    }
}