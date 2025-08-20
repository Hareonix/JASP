using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Client
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    public List<string> Tags { get; set; } = new();

    public bool IsOnline { get; set; }

    public DateTime? LastSeenAt { get; set; }

    public string? Notes { get; set; }

    public List<ClientOutbound> Outbounds { get; set; } = new();
}

