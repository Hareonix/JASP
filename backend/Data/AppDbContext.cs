using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Inbound> Inbounds => Set<Inbound>();
    public DbSet<Outbound> Outbounds => Set<Outbound>();
    public DbSet<ClientOutbound> ClientOutbounds => Set<ClientOutbound>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var tagsConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());

        modelBuilder.Entity<Client>()
            .Property(c => c.Tags)
            .HasConversion(tagsConverter);

        modelBuilder.Entity<ClientOutbound>()
            .HasKey(co => new { co.ClientId, co.OutboundId });

        modelBuilder.Entity<ClientOutbound>()
            .HasOne(co => co.Client)
            .WithMany(c => c.Outbounds)
            .HasForeignKey(co => co.ClientId);

        modelBuilder.Entity<ClientOutbound>()
            .HasOne(co => co.Outbound)
            .WithMany(o => o.Clients)
            .HasForeignKey(co => co.OutboundId);

        base.OnModelCreating(modelBuilder);
    }
}

