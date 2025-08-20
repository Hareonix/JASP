using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IEnumerable<User> Get() => _context.Users.ToList();

    [HttpGet("{id:int}")]
    public ActionResult<User> Get(int id)
    {
        var user = _context.Users.Find(id);
        return user is null ? NotFound() : user;
    }

    [HttpPost]
    public ActionResult<User> Post(User user)
    {
        user.CreatedAt = DateTime.UtcNow;
        _context.Users.Add(user);
        _context.SaveChanges();
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }
}

