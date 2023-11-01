using gerenciadorTarefa.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace gerenciadorTarefa.Controllers
{
    //[Authorize]
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;

        public UsuariosController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            AppDbContext context,
            EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
        }
        //Index
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        //Login

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Usuario usuario)
        {
            var user = await _userManager.FindByEmailAsync(usuario.Email);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, usuario.Senha, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Usuário e/ou senha inválidos.");
            return View();
        }

        //Logout
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Usuarios");
        }

        //Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.FindByIdAsync(id.ToString());

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }
            
        //Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("Id, Name, Email, Senha, ConfirmarSenha")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (usuario.Senha != usuario.ConfirmarSenha)
                {
                    ModelState.AddModelError("ConfirmarSenha", "A senha e a confirmação de senha não coincidem.");
                    return View(usuario);
                }

                var existingUser = await _userManager.FindByEmailAsync(usuario.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Este email já está em uso.");
                    return View(usuario);
                }

                var user = new IdentityUser
                {
                    UserName = usuario.Name,
                    Email = usuario.Email
                };

                var result = await _userManager.CreateAsync(user, usuario.Senha);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Cadastro criado com sucesso! Realize login para iniciar.";
                    return RedirectToAction("Login", "Usuarios");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(usuario);
        }

        //Edit
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var usuario = new Usuario
            {
                Name = user.UserName,
                Email = user.Email
            };

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser == null)
                {
                    return NotFound();
                }

                currentUser.UserName = usuario.Name; 
                currentUser.Email = usuario.Email; 

                var result = await _userManager.UpdateAsync(currentUser);

                if (result.Succeeded)
                {
                    var userClaims = await _userManager.GetClaimsAsync(currentUser);
                    var nameClaim = userClaims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name);

                    if (nameClaim != null)
                    {
                        await _userManager.RemoveClaimAsync(currentUser, nameClaim);
                    }

                    await HttpContext.SignOutAsync();
                    TempData["SuccessMessage"] = "Perfil atualizado com sucesso. Faça login novamente.";
                    return RedirectToAction("Login", "Usuarios");

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(usuario);
        }



        //Delete
        [HttpGet]
        public async Task<IActionResult> Delete(Usuario usuario)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Usuario usuario)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Usuarios");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(usuario);
        }



        // Esqueci Minha Senha
        [AllowAnonymous]
        public IActionResult EsqueciMinhaSenha()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> EsqueciMinhaSenha(EsqueciMinhaSenhaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("Email", "E-mail não encontrado.");
                    return View(model);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("RedefinirSenha", "Usuarios", new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

                var emailService = new EmailService(_configuration); // Injete a IConfiguration no construtor do EmailService
                emailService.SendEmailAsync(model.Email, "Redefinição de Senha", "Clique no seguinte link para redefinir sua senha: " + callbackUrl);

                TempData["SuccessMessage"] = "Link para recuperação de senha enviado para o e-mail.";
                return RedirectToAction("Login", "Usuarios");
            }

            return View(model);
        }


        [AllowAnonymous]
        public IActionResult RedefinirSenha(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest();
            }

            var model = new RedefinirSenhaViewModel
            {
                UserId = userId,
                Token = token
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> RedefinirSenha(RedefinirSenhaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);

                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Senha);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Login", "Usuarios");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
                }
            }

            return View(model);
        }



    }
}
