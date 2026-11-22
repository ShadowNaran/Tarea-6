using CatalogoApi.Data;
using CatalogoApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogoApi.Controllers;

[ApiController]
[Route("api/resenas")]
[Authorize]
public class ResenasController : ControllerBase
{
    private readonly AppDbContext _context;

    public ResenasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var resenas = await _context.Resenas
            .Where(r => r.Activo)
            .ToListAsync();
        return Ok(resenas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var resena = await _context.Resenas
            .FirstOrDefaultAsync(r => r.Id == id && r.Activo);

        if (resena == null)
        {
            return NotFound(new { mensaje = $"Reseña con ID {id} no encontrada o no se encuentra activa." });
        }

        return Ok(resena);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Resena resena)
    {
        if (string.IsNullOrWhiteSpace(resena.Autor) ||
            string.IsNullOrWhiteSpace(resena.TipoContenido) ||
            string.IsNullOrWhiteSpace(resena.TituloObra))
        {
            return BadRequest(new { mensaje = "Autor, tipo_contenido y titulo_obra son obligatorios." });
        }

        if (resena.Puntuacion < 1 || resena.Puntuacion > 5)
        {
            return BadRequest(new { mensaje = "La puntuación debe estar entre 1 y 5." });
        }

        resena.Activo = true;
        resena.CreadoEn = DateTime.UtcNow;
        resena.EliminadoEn = null;

        _context.Resenas.Add(resena);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = resena.Id }, resena);
    }

    // Endpoint duplicado intencionalmente para la PR de práctica (Etapa 4)
    [HttpPost]
    public async Task<IActionResult> CreateDuplicado([FromBody] Resena resena)
    {
        resena.Activo = true;
        resena.CreadoEn = DateTime.UtcNow;
        resena.EliminadoEn = null;

        _context.Resenas.Add(resena);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = resena.Id }, resena);
    }
}
