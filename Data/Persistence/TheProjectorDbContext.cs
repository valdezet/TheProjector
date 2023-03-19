using Microsoft.EntityFrameworkCore;


namespace TheProjector.Data.Persistence;

public class TheProjectorDbContext : DbContext
{
    public DbSet<Project> Projects { get; set; }

    public DbSet<Person> People { get; set; }

    public DbSet<PersonProjectAssignment> PersonProjectAssignments { get; set; }

    public TheProjectorDbContext(DbContextOptions<TheProjectorDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
        .HasMany(project => project.AssignedPeople)
        .WithMany(person => person.AssignedProjects)
        .UsingEntity<PersonProjectAssignment>(
            j => j.HasOne(ppa => ppa.Person)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict),
            j => j.HasOne(ppa => ppa.Project)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict)
        );
    }
}