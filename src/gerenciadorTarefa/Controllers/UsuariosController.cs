using gerenciadorTarefa.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using IEmailService = gerenciadorTarefa.Services.IEmailService;

namespace gerenciadorTarefa.Controllers
{
    //[Authorize]
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UsuariosController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            AppDbContext context,
            IEmailService emailService)
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
                    return RedirectToAction("Index", "Meta");
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
                    var token = await _userManager.GeneratePasswordResetTokenAsync(currentUser);
                    var resetResult = await _userManager.ResetPasswordAsync(currentUser, token, usuario.Senha);

                    if (resetResult.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Perfil atualizado com sucesso. Faça login novamente.";
                        return RedirectToAction("Login", "Usuarios");
                    }
                    else
                    {
                        foreach (var error in resetResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
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
                if(_userManager.Users.AsNoTracking().Any(u => u.NormalizedEmail == model.Email.ToUpper().Trim()))
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var urlConfirmacao = Url.Action(nameof(RedefinirSenha), "Usuarios", new { token }, Request.Scheme);
                    var mensagem = new StringBuilder();
                    mensagem.Append($"<p>Olá,</p>");
                    mensagem.Append($"<p>Houve uma solicitação de redefinição de senha para seu usuário em nosso site. Se não foi você quem realizou a solicitação, gentileza desconsiderar essa mensagem. </br> Caso tenha sido você, clique no link abaixo para criar sua nova senha:</p>");


                    mensagem.Append($"<p><a href='{urlConfirmacao}'>Redefinir Senha</a></p>");
                    mensagem.Append($"<p>Atenciosamente,<br>Equipe de Suporte</p>");
                    await _emailService.SendEmailAsync(model.Email,
                                                       "Redefinição de Senha",
                                                       "",
                                                       mensagem.ToString());
                    return View(nameof(EmailRedefinicaoEnviado));

                }
                else
                {
                    ModelState.AddModelError(string.Empty, 
                        $"Usuário/e-mail <b>{model.Email}</b>não encontrado.");
                    return View();
                }
            }
            else
            {
                return View(model);
            }
        }

        [AllowAnonymous]
        public IActionResult EmailRedefinicaoEnviado()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult RedefinirSenha(string token)
        {
            var modelo = new RedefinirSenhaViewModel();
            modelo.Token = token;
            return View(modelo);
        }
       
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RedefinirSenha(RedefinirSenhaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var resultado = await _userManager.ResetPasswordAsync(
                    user, model.Token, model.NovaSenha);

                if (resultado.Succeeded)
                {
                    TempData["SuccessMessage"] = "Senha redefinida com sucesso! Agora você já pode fazer login com a nova senha.";
                    return View(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Não foi possível redefinir senha. Verifique se preencheu a senha corretamente. Se o problema persistir, entre em contato com o suporte.");
                    return View(model);
                }
            }

            return View(model);
        }

    }
}
