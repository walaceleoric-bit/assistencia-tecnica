using AssistenciaTecnica.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Controllers
{
    public class AreaClienteController : Controller
    {
        private readonly AppDbContext _context;

        public AreaClienteController(AppDbContext context)
        {
            _context = context;
        }

        private bool ClienteLogado()
        {
            return HttpContext.Session.GetString("CLIENTE_LOGADO") == "SIM";
        }

        private int ClienteId()
        {
            return HttpContext.Session.GetInt32("CLIENTE_ID") ?? 0;
        }

        private int EmpresaId()
        {
            return HttpContext.Session.GetInt32("CLIENTE_EMPRESA_ID") ?? 0;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string usuario, string senha)
        {
            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(senha))
            {
                TempData["Erro"] = "Digite usuário e senha.";
                return RedirectToAction("Login");
            }

            usuario = usuario.Trim().ToLower();
            senha = senha.Trim();

            var clientes = await _context.Clientes.ToListAsync();

            var cliente = clientes.FirstOrDefault(c =>
                (c.UsuarioCliente ?? "").Trim().ToLower() == usuario &&
                (c.SenhaCliente ?? "").Trim() == senha);

            if (cliente == null)
            {
                TempData["Erro"] = "Usuário ou senha inválidos.";
                return RedirectToAction("Login");
            }

            HttpContext.Session.Clear();

            HttpContext.Session.SetString("CLIENTE_LOGADO", "SIM");
            HttpContext.Session.SetInt32("CLIENTE_ID", cliente.Id);
            HttpContext.Session.SetInt32("CLIENTE_EMPRESA_ID", cliente.EmpresaId);
            HttpContext.Session.SetString("CLIENTE_NOME", cliente.Nome);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index()
        {
            if (!ClienteLogado())
            {
                return RedirectToAction("Login");
            }

            var clienteId = ClienteId();
            var empresaId = EmpresaId();

            var ordens = await _context.OrdensServico
                .Include(o => o.Servico)
                .Where(o =>
                    o.ClienteId == clienteId &&
                    o.EmpresaId == empresaId)
                .OrderByDescending(o => o.DataAbertura)
                .ToListAsync();

            return View(ordens);
        }

        public IActionResult Sair()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Landing", "Home");
        }
    }
}