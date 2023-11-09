using gerenciadorTarefa.Models;
using gerenciadorTarefa.Models.ViewModel;
using gerenciadorTarefa.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace gerenciadorTarefa.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IEmailService emailService, AppDbContext context)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            var metaViewModels = _context.Metas.Select(meta => new MetaViewModel
            {
                // Map properties from your Meta entity to MetaViewModel
                Categoria = meta.Categoria,
                Titulo = meta.Titulo,
                // ... map other properties
            }).ToList();

            return View(metaViewModels);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> EnviarEmailTeste()
        {
            var html = new StringBuilder();
            html.Append("<h1>Teste de Serviço de Envio de E-mail</h1>");
            html.Append("<p>Este é um teste do serviço de envio de e-mails usando ASP.NET Core.</p>");
            await _emailService.SendEmailAsync("gerenciadordetarefasorganizer@gmail.com", "Teste de Serviço de Email", string.Empty, html.ToString());
            TempData["SuccessMessage"] = "Uma mensagem foi enviada para o e-mail gerenciadordetarefasorganizer@gmail.com.";
            return RedirectToAction(nameof(Index));
        }
    }
}
