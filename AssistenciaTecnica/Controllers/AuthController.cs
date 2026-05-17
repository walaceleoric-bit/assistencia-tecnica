using AssistenciaTecnica.Data;
using AssistenciaTecnica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(
            string usuario,
            string senha)
        {
            var config = await _context.Configuracoes
                .FirstOrDefaultAsync();

            if (config == null)
            {
                config = new Configuracao();
                _context.Configuracoes.Add(config);
                await _context.SaveChangesAsync();
            }

            var senhaAdm = string.IsNullOrWhiteSpace(config.SenhaAdm)
                ? "123456"
                : config.SenhaAdm;

            if (usuario == "admin" &&
                senha == senhaAdm)
            {
                HttpContext.Session
                    .SetString("ADM_LOGADO", "SIM");

                return RedirectToAction("Index", "Admin");
            }

            TempData["Erro"] =
                "Usuário ou senha inválidos.";

            return RedirectToAction("Login");
        }

        public IActionResult Sair()
        {
            HttpContext.Session
                .Remove("ADM_LOGADO");

            return RedirectToAction("Index", "Home");
        }
    }
}