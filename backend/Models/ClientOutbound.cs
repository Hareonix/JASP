using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class ClientOutbound
{
    [Required]
    public int ClientId { get; set; }

    [Required]
    public int OutboundId { get; set; }

    public int Priority { get; set; }

    public bool Enabled { get; set; }

    public Client? Client { get; set; }
    public Outbound? Outbound { get; set; }
}

