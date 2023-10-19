using gerenciadorTarefa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;
using gerenciadorTarefa.Models.ViewModel;

namespace gerenciadorTarefa.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class MetasController : Controller

    {
        private readonly AppDbContext _context;


        public MetasController(AppDbContext context)
        {
            _context = context;
        }


        [Authorize]
        public async Task<IActionResult> Index()
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            var metas = await _context.Metas
                .Include(m => m.Usuario) 
                    .Where(m => m.Usuario.Id.ToString() == userId)
                .ToListAsync();

            return View(metas);
        }


        //GET
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Meta meta)
        {
            if (ModelState.IsValid)
            {
                _context.Metas.Add(meta);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id","Name", meta.UsuarioId);
            return View(meta);
        }





        public IActionResult CreateMeta()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id");
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMeta(MetaViewModel metaViewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = (); // Obtenha o ID do usuário logado

                if (userId != null)
                {
                    var meta = new Meta
                    {
                        Categoria = metaViewModel.Categoria,
                        Titulo = metaViewModel.Titulo,
                        Prazo = metaViewModel.Prazo,
                        UsuarioId = userId, // Defina o UsuarioId com o ID do usuário logado
                    };

                    _context.Metas.Add(meta);
                    await _context.SaveChangesAsync();

                    var tarefa = new Tarefa
                    {
                        Nome = metaViewModel.Nome,
                        Status = metaViewModel.Status,
                        Id = userId,
                        MetasId = meta.Id,
                    };

                    _context.Tarefas.Add(tarefa);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                else
                {
                    // Lida com o caso em que o usuário não está autenticado, como redirecionar para a página de login
                    return RedirectToAction("Login", "Usuarios");
                }
            }

            return View(metaViewModel);
        }

    }

}


