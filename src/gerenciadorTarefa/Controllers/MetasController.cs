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
        return View(new List<MetaViewModel>());

    }
    public IActionResult Create()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userId))
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
    public async Task<IActionResult> Create(MetaViewModel metaViewModel)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
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
                return RedirectToAction("Index", "Home");
            }
        }

        return View(metaViewModel);
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