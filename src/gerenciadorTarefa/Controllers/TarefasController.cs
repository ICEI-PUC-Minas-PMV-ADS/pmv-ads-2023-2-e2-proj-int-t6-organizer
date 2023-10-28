using gerenciadorTarefa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace gerenciadorTarefa.Controllers
{
    public class TarefasController : Controller
    {
        private readonly AppDbContext _context;

        public TarefasController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dados = await _context.Tarefas.ToListAsync();

            return View(dados);
        }

        //GET
        public IActionResult Create()
        {
            ViewData["MetaId"] = new SelectList(_context.Metas, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Tarefa tarefa)
        {
            if (ModelState.IsValid)
            {
                _context.Tarefas.Add(tarefa);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["MetaId"] = new SelectList(_context.Metas, "Id", "Name", tarefa.MetasId);
            return View(tarefa);
        }
    }
}