namespace Backend.Dtos;

public class ClientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public bool IsOnline { get; set; }
    public DateTime? LastSeenAt { get; set; }
    public string? Notes { get; set; }
}

