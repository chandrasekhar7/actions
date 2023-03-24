namespace Npa.Accounting.Infrastructure.Repay;

public record ErrorItem
{
    public string Location { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    private ErrorItem()
    {
        
    }

    public ErrorItem(string location, string name, string description)
    {
        Location = location;
        Name = name;
        Description = description;
    }

    public override string ToString() => $"Error ({Location}) - {Name}: {Description}";
}