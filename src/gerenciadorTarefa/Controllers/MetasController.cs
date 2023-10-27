using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gerenciadorTarefa.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using gerenciadorTarefa.Models.ViewModel;
using Microsoft.AspNetCore.Identity;

public class MetaController : Controller
{
    private readonly AppDbContext _context;

    public MetaController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var userIdAsString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userIdAsString, out int userId))
        {
            var metas = _context.Metas
                .Where(m => m.UsuarioId == userId)
                .Include(m => m.Tarefas)
                .ToList();

            var metaViewModels = metas.Select(meta => new MetaViewModel
            {
                Id = meta.Id,
                Categoria = meta.Categoria,
                Titulo = meta.Titulo,
                Prazo = meta.Prazo,
                Status = meta.Status,
                DataRegistro = meta.DataRegistro,
                UsuarioId = meta.UsuarioId,
                Tarefas = meta.Tarefas.Select(tarefa => new TarefaViewModel
                {
                    Id = tarefa.Id,
                    Nome = tarefa.Nome,
                    Status = tarefa.Status,
                }).ToList()
            }).ToList();

            return View(metaViewModels);
        }
        else
        {
            return View(new List<MetaViewModel>());
        }
    }

    public IActionResult CreateMeta()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userIdString, out var userId))
        {
            var novaMetaViewModel = new MetaViewModel
            {
                UsuarioId = userId
            };

            return View(novaMetaViewModel);
        }

        return RedirectToAction("ErrorAction");
    }


    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> CreateMeta(MetaViewModel metaViewModel)

    {
        if (ModelState.IsValid)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdString, out int userId))
            {

                var meta = new Meta
                {
                    Categoria = metaViewModel.Categoria,
                    Titulo = metaViewModel.Titulo,
                    Prazo = metaViewModel.Prazo,
                    Status = metaViewModel.Status,
                    DataRegistro = metaViewModel.DataRegistro,
                    UsuarioId = userId
                };
                _context.Metas.Add(meta);
                await _context.SaveChangesAsync();

                foreach (var tarefaViewModel in metaViewModel.Tarefas)
                {
                    var tarefa = new Tarefa
                    {
                        Nome = tarefaViewModel.Nome,
                        Status = tarefaViewModel.Status,
                        MetasId = meta.Id

                    };
                    _context.Tarefas.Add(tarefa);

                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
        }
        return View(metaViewModel);
    }

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
