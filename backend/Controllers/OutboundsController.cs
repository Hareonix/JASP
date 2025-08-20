using AutoMapper;
using Backend.Data;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutboundsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public OutboundsController(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<OutboundDto>> Get()
        => _mapper.Map<List<OutboundDto>>(await _db.Outbounds.ToListAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<OutboundDto>> Get(int id)
    {
        var outbound = await _db.Outbounds.FindAsync(id);
        if (outbound == null) return NotFound();
        return _mapper.Map<OutboundDto>(outbound);
    }

    [HttpPost]
    public async Task<ActionResult<OutboundDto>> Create(OutboundDto dto)
    {
        var outbound = _mapper.Map<Outbound>(dto);
        outbound.CreatedAt = outbound.UpdatedAt = DateTime.UtcNow;
        _db.Outbounds.Add(outbound);
        await _db.SaveChangesAsync();
        dto.Id = outbound.Id;
        dto.CreatedAt = dto.UpdatedAt = outbound.CreatedAt;
        return CreatedAtAction(nameof(Get), new { id = outbound.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, OutboundDto dto)
    {
        var outbound = await _db.Outbounds.FindAsync(id);
        if (outbound == null) return NotFound();
        _mapper.Map(dto, outbound);
        outbound.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var outbound = await _db.Outbounds.FindAsync(id);
        if (outbound == null) return NotFound();
        _db.Outbounds.Remove(outbound);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

