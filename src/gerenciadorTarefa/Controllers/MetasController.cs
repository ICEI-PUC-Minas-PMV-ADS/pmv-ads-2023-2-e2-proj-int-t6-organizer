using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gerenciadorTarefa.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

public class MetaController : Controller
{
    private readonly AppDbContext _context;

    public MetaController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Meta
    public async Task<IActionResult> Index()
    {
        var metas = await _context.Metas.Include(m => m.Usuario).ToListAsync();
        return View(metas);
    }

    // GET: Meta/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var meta = await _context.Metas
            .Include(m => m.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meta == null)
        {
            return NotFound();
        }

        return View(meta);
    }

    // GET: Meta/Create
    public IActionResult Create()
    {
        ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Name");
        return View();
    }

    // POST: Meta/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Categoria,Titulo,Prazo,Status,DataRegistro,UsuarioId")] Meta meta)
    {
        if (ModelState.IsValid)
        {
            _context.Add(meta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Name", meta.UsuarioId);
        return View(meta);
    }

    // GET: Meta/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var meta = await _context.Metas.FindAsync(id);
        if (meta == null)
        {
            return NotFound();
        }

        ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Name", meta.UsuarioId);
        return View(meta);
    }

    // POST: Meta/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Categoria,Titulo,Prazo,Status,DataRegistro,UsuarioId")] Meta meta)
    {
        if (id != meta.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(meta);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MetaExists(meta.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Name", meta.UsuarioId);
        return View(meta);
    }

    // GET: Meta/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var meta = await _context.Metas
            .Include(m => m.Usuario)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meta == null)
        {
            return NotFound();
        }

        return View(meta);
    }

    // POST: Meta/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var meta = await _context.Metas.FindAsync(id);
        _context.Metas.Remove(meta);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MetaExists(int id)
    {
        return _context.Metas.Any(e => e.Id == id);
    }
}
