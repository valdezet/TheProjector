using Microsoft.EntityFrameworkCore;


namespace TheProjector.Data.Persistence;

public class TheProjectorDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }

    public DbSet<Person> People { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<User> Users { get; set; }

    public TheProjectorDbContext(DbContextOptions<TheProjectorDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasMany(project => project.AssignedPeople)
            .WithMany(person => person.AssignedProjects)
            .UsingEntity<Dictionary<string, object>>(
                "PersonProject",
                x => x.HasOne<Person>().WithMany().OnDelete(DeleteBehavior.Restrict),
                x => x.HasOne<Project>().WithMany().OnDelete(DeleteBehavior.Restrict)
            );

        modelBuilder.Entity<Role>()
            .HasData(new Role[] {
                new Role{Id = 1, Name = "Super Admin"},
                new Role{Id = 2, Name = "Administrator"}
            });
    }
}