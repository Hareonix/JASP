using AutoMapper;
using Backend.Data;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ClientsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<ClientDto>> Get()
        => _mapper.Map<List<ClientDto>>(await _db.Clients
            .Include(c => c.Outbounds)
            .ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ClientDto>> Get(int id)
    {
        var client = await _db.Clients
            .Include(c => c.Outbounds)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (client == null) return NotFound();
        return _mapper.Map<ClientDto>(client);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(ClientDto dto)
    {
        var client = _mapper.Map<Client>(dto);
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        dto.Id = client.Id;
        return CreatedAtAction(nameof(Get), new { id = client.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ClientDto dto)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null) return NotFound();
        _mapper.Map(dto, client);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/outbounds")]
    public async Task<IActionResult> SetOutbounds(int id, List<ClientOutboundDto> dtos)
    {
        var client = await _db.Clients
            .Include(c => c.Outbounds)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (client == null) return NotFound();

        _db.ClientOutbounds.RemoveRange(client.Outbounds);
        foreach (var dto in dtos)
        {
            _db.ClientOutbounds.Add(new ClientOutbound
            {
                ClientId = id,
                OutboundId = dto.OutboundId,
                Priority = dto.Priority,
                Enabled = dto.Enabled
            });
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null) return NotFound();
        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

