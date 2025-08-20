using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Inbound
{
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Type { get; set; }

    public string Version { get; set; } = "";

    [Required]
    public required string JsonConfig { get; set; }

    public bool Enabled { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

