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

        public async Task<IActionResult> Login()
        {
            await CriarEmpresaPadraoSeNaoExistir();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string senha)
        {
            await CriarEmpresaPadraoSeNaoExistir();

            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(senha))
            {
                TempData["Erro"] = "Digite usuário e senha.";
                return RedirectToAction("Login");
            }

            // LOGIN DO DONO WLC
            if (usuario == "wlc" && senha == "123456")
            {
                HttpContext.Session.Clear();

                HttpContext.Session.SetString("DONO_LOGADO", "SIM");

                return RedirectToAction("Index", "Dono");
            }

            // LOGIN DAS EMPRESAS / INQUILINOS
            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(e =>
                    e.Usuario == usuario &&
                    e.Senha == senha &&
                    e.Ativo);

            if (empresa == null)
            {
                TempData["Erro"] = "Usuário ou senha inválidos.";
                return RedirectToAction("Login");
            }

            HttpContext.Session.Clear();

            HttpContext.Session.SetString("ADM_LOGADO", "SIM");
            HttpContext.Session.SetInt32("EMPRESA_ID", empresa.Id);
            HttpContext.Session.SetString("EMPRESA_NOME", empresa.NomeEmpresa);

            return RedirectToAction("Index", "Admin");
        }

        public IActionResult Sair()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Landing", "Home");
        }

        private async Task CriarEmpresaPadraoSeNaoExistir()
        {
            var existeEmpresa = await _context.Empresas.AnyAsync();

            if (!existeEmpresa)
            {
                var empresa = new Empresa
                {
                    NomeEmpresa = "Milton Cardoso",
                    Usuario = "admin",
                    Senha = "123456",
                    WhatsApp = "",
                    Ativo = true,
                    DataCadastro = DateTime.UtcNow
                };

                _context.Empresas.Add(empresa);

                await _context.SaveChangesAsync();
            }
        }
    }
}