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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userId))
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


    public IActionResult Edit(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("ErrorAction");
        }

        var meta = _context.Metas
            .Include(m => m.Tarefas)
            .SingleOrDefault(m => m.Id == id && m.UsuarioId == userId);

        if (meta == null)
        {
            return NotFound();
        }

        var metaViewModel = new MetaViewModel
        {
            Categoria = meta.Categoria,
            Titulo = meta.Titulo,
            Prazo = meta.Prazo,
            Status = meta.Status,
            DataRegistro = meta.DataRegistro,
            UsuarioId = userId,
            Tarefas = meta.Tarefas.Select(t => new TarefaViewModel
            {
                Id = t.Id,
                Nome = t.Nome,
                Status = t.Status
            }).ToList()
        };

        return View(metaViewModel);
    }

    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Edit(int id, MetaViewModel metaViewModel)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("ErrorAction");
            }

            var meta = await _context.Metas
                .Include(m => m.Tarefas)
                .SingleOrDefaultAsync(m => m.Id == id && m.UsuarioId == userId);

            if (meta == null)
            {
                return NotFound();
            }

            meta.Categoria = metaViewModel.Categoria;
            meta.Titulo = metaViewModel.Titulo;
            meta.Prazo = metaViewModel.Prazo;
            meta.Status = metaViewModel.Status;
            meta.DataRegistro = metaViewModel.DataRegistro;

            meta.Tarefas ??= new List<Tarefa>();  // Inicializa se for nulo

            foreach (var tarefaViewModel in metaViewModel.Tarefas)
            {
                var tarefa = meta.Tarefas.SingleOrDefault(t => t.Id == tarefaViewModel.Id);

                if (tarefa != null)
                {
                    tarefa.Nome = tarefaViewModel.Nome;
                    tarefa.Status = tarefaViewModel.Status;
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(metaViewModel);
    }




    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("ErrorAction");
        }

        var meta = await _context.Metas
            .Include(m => m.Tarefas)
            .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == userId);

        if (meta == null)
        {
            return NotFound();
        }

        return View("Delete", meta);
    }



    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("ErrorAction");
        }

        var meta = await _context.Metas
            .Include(m => m.Tarefas)
            .FirstOrDefaultAsync(m => m.Id == id && m.UsuarioId == userId);

        if (meta == null)
        {
            return NotFound();
        }

        _context.Metas.Remove(meta);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }


}