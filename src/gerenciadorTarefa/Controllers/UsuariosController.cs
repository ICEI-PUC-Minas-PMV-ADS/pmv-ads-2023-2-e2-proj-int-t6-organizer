using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using gerenciadorTarefa.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;


namespace gerenciadorTarefa.Controllers
{
    //[Authorize]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }
        //- DESENVOLVENDO
        /*
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public UsuariosController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        */


        //------------------------- GET: Index
        public async Task<IActionResult> Index()
        {
              return View(await _context.Usuarios.ToListAsync());
        }

        //------------------------- GET: Login

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        //------------------------- POST: Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login (Usuario usuario)
        {
            var dados = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);

            if (dados != null)
            {
                bool senhaOk = BCrypt.Net.BCrypt.Verify(usuario.Senha, dados.Senha);

                if (!senhaOk)
                {
                    ViewBag.Message = "Usuário e/ou senha inválidos!";
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, dados.Name),
                        new Claim(ClaimTypes.NameIdentifier, dados.Id.ToString()),
                        new Claim(ClaimTypes.Email, dados.Email)
                    };

                    var usuarioIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal principal = new ClaimsPrincipal(usuarioIdentity);

                    var props = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        ExpiresUtc = DateTime.UtcNow.ToLocalTime().AddHours(8),
                        IsPersistent = true,
                    };

                    await HttpContext.SignInAsync(principal, props);

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Message = "Usuário e/ou senha inválidos!";
            }
            return View();
        }

        //------------------------- GET: Logout
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Usuarios");
        }

        //------------------------- GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        //------------------------- GET: Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        //------------------------- POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Senha,ConfirmarSenha")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (usuario.Senha != usuario.ConfirmarSenha)
                {
                    ModelState.AddModelError("ConfirmarSenha", "A senha e a confirmação de senha não coincidem.");
                    return View(usuario);
                }

                if (_context.Usuarios.Any(u => u.Email == usuario.Email))
                {
                    ModelState.AddModelError("Email", "Este email já está em uso.");
                    return View(usuario);
                }

                
                usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cadastro criado com sucesso! Realize login para iniciar.";

                return RedirectToAction("Login", "Usuarios");
            }
            return View(usuario);
        }

        //------------------------- GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        //------------------------- POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Senha,ConfirmarSenha")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (usuario.Senha != usuario.ConfirmarSenha)
                {
                    ModelState.AddModelError("ConfirmarSenha", "A senha e a confirmação de senha não coincidem.");
                    return View(usuario);
                }

                try
                {
                    usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
                    _context.Update(usuario);

                    await _context.SaveChangesAsync();
                    await HttpContext.SignOutAsync();
                    return RedirectToAction("Login", "Usuarios");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(usuario);
        }

        //------------------------- GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        //------------------------- POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'AppDbContext.Usuarios'  is null.");
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
            
            await _context.SaveChangesAsync();

            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Usuarios");
        }

        private bool UsuarioExists(int id)
        {
          return _context.Usuarios.Any(e => e.Id == id);
        }

      
        //------------------------- GET: Esqueci Minha Senha
        [AllowAnonymous]
        public IActionResult EsqueciMinhaSenha()
        {
            return View();
        }

        //------------------------- DESENVOLVENDO

        //------------------------- POST: Enviar Link Recuperacao
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarLinkRecuperacao(string Email)
        {
          var usuario = await _userManager.FindByEmailAsync(Email);

          if (usuario == null || !(await _userManager.IsEmailConfirmedAsync(usuario)))
          {
              // Trate o cenário em que o e-mail não foi encontrado ou não foi confirmado
              ModelState.AddModelError("Email", "E-mail não encontrado ou não confirmado.");
              return View();
          }

          var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
          var callbackUrl = Url.Action("RedefinirSenha", "Usuarios", new { userId = usuario.Id, token = token }, protocol: HttpContext.Request.Scheme);
          //DESENVOLVENDO
          //await _emailSender.SendEmailAsync(Email, "Redefinir sua senha", callbackUrl);

          // Redirecione para uma página informando ao usuário que o link foi enviado com sucesso
          return View("LinkRecuperacaoEnviado");
        }

        //------------------------- POST: Redefinir Senha - DESENVOLVENDO
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

        //------------------------- POST: Salvar Nova Senha - DESENVOLVENDO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarNovaSenha(RedefinirSenhaViewModel model)
        {
           if (ModelState.IsValid)
           {
               var usuario = await _userManager.FindByIdAsync(model.UserId);

               if (usuario != null)
               {
                   var result = await _userManager.ResetPasswordAsync(usuario, model.Token, model.Senha);

                   if (result.Succeeded)
                   {
                       // Redefinição de senha bem-sucedida, redirecione o usuário para a página de login ou outra página apropriada
                       return RedirectToAction("Login", "Usuarios");
                   }
                   else
                   {
                       // Trate os erros de redefinição de senha, se houverem
                       foreach (var error in result.Errors)
                       {
                           ModelState.AddModelError(string.Empty, error.Description);
                       }
                   }
               }
               else
               {
                   // Trate o cenário em que o usuário não é encontrado
                   ModelState.AddModelError(string.Empty, "Usuário não encontrado.");
               }
           }

           return View();
        }
         */
    }
}
