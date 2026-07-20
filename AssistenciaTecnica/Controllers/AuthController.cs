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

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
            {
                TempData["Erro"] = "Digite usuário e senha.";
                return RedirectToAction("Login");
            }

            var usuarioDigitado = usuario.Trim().ToLower();
            var senhaDigitada = senha.Trim();

            // 1. Dono do sistema
            if (usuarioDigitado == "wlc" && senhaDigitada == "123456")
            {
                HttpContext.Session.Clear();
                HttpContext.Session.SetString("DONO_LOGADO", "SIM");

                return RedirectToAction("Index", "Dono");
            }

            // 2. Empresa / Admin
            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(e =>
                    e.Usuario.ToLower() == usuarioDigitado &&
                    e.Senha == senhaDigitada &&
                    e.Ativo);

            if (empresa != null)
            {
                HttpContext.Session.Clear();

                HttpContext.Session.SetString("ADM_LOGADO", "SIM");
                HttpContext.Session.SetInt32("EMPRESA_ID", empresa.Id);
                HttpContext.Session.SetString("EMPRESA_NOME", empresa.NomeEmpresa);

                return RedirectToAction("Index", "Admin");
            }

            // 3. Cliente (Busca otimizada no banco de dados)
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c =>
                    c.UsuarioCliente != null && c.UsuarioCliente.Trim().ToLower() == usuarioDigitado &&
                    c.SenhaCliente != null && c.SenhaCliente.Trim() == senhaDigitada);

            if (cliente != null)
            {
                HttpContext.Session.Clear();

                var empresaDoCliente = await _context.Empresas
                    .FirstOrDefaultAsync(e => e.Id == cliente.EmpresaId);

                HttpContext.Session.SetString("CLIENTE_LOGADO", "SIM");
                HttpContext.Session.SetInt32("CLIENTE_ID", cliente.Id);
                HttpContext.Session.SetInt32("CLIENTE_EMPRESA_ID", cliente.EmpresaId);
                HttpContext.Session.SetInt32("EMPRESA_ID", cliente.EmpresaId);

                HttpContext.Session.SetString("CLIENTE_NOME", cliente.Nome);

                HttpContext.Session.SetString(
                    "EMPRESA_NOME",
                    empresaDoCliente?.NomeEmpresa ?? "Empresa"
                );

                return RedirectToAction("Index", "Home");
            }

            TempData["Erro"] = "Usuário ou senha inválidos.";
            return RedirectToAction("Login");
        }

        public IActionResult Sair()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Landing", "Home");
        }

        private async Task CriarEmpresaPadraoSeNaoExistir()
        {
            try
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
            catch (Exception ex)
            {
                // Registra o log no console caso ocorra alguma falha na criação da empresa
                Console.WriteLine($"Erro ao verificar/criar empresa padrão: {ex.Message}");
            }
        }
    }
}