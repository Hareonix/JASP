using AutoMapper;
using Backend.Data;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InboundsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public InboundsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<InboundDto>> Get()
        => _mapper.Map<List<InboundDto>>(await _db.Inbounds.ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<InboundDto>> Get(int id)
    {
        var inbound = await _db.Inbounds.FindAsync(id);
        if (inbound == null) return NotFound();
        return _mapper.Map<InboundDto>(inbound);
    }

    [HttpPost]
    public async Task<ActionResult<InboundDto>> Create(InboundDto dto)
    {
        var inbound = _mapper.Map<Inbound>(dto);
        inbound.CreatedAt = inbound.UpdatedAt = DateTime.UtcNow;
        _db.Inbounds.Add(inbound);
        await _db.SaveChangesAsync();
        dto.Id = inbound.Id;
        dto.CreatedAt = dto.UpdatedAt = inbound.CreatedAt;
        return CreatedAtAction(nameof(Get), new { id = inbound.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, InboundDto dto)
    {
        var inbound = await _db.Inbounds.FindAsync(id);
        if (inbound == null) return NotFound();
        _mapper.Map(dto, inbound);
        inbound.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var inbound = await _db.Inbounds.FindAsync(id);
        if (inbound == null) return NotFound();
        _db.Inbounds.Remove(inbound);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

