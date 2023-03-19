namespace TheProjector.Data.Persistence;

public class PersonProjectAssignment
{
    public long PersonId { get; set; }
    public Person Person { get; set; }

    public long ProjectId { get; set; }
    public Project Project { get; set; }
}