using AssistenciaTecnica.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var admLogado = HttpContext.Session.GetString("ADM_LOGADO") == "SIM";
            var clienteLogado = HttpContext.Session.GetString("CLIENTE_LOGADO") == "SIM";

            // Se ninguém estiver autenticado na sessão, redireciona para a tela de Login
            if (!admLogado && !clienteLogado)
            {
                return RedirectToAction("Login", "Auth");
            }

            var empresaId = HttpContext.Session.GetInt32("EMPRESA_ID")
                ?? HttpContext.Session.GetInt32("CLIENTE_EMPRESA_ID")
                ?? 0;

            // Busca a configuração da empresa para exibir na Home (inclusive as imagens de Destaque)
            var config = await _context.Configuracoes
                .FirstOrDefaultAsync(c => c.EmpresaId == empresaId);

            return View(config);
        }

        public async Task<IActionResult> Contato()
        {
            var empresaId = HttpContext.Session.GetInt32("EMPRESA_ID")
                ?? HttpContext.Session.GetInt32("CLIENTE_EMPRESA_ID")
                ?? 0;

            var config = await _context.Configuracoes
                .FirstOrDefaultAsync(c => c.EmpresaId == empresaId);

            return View(config);
        }

        public async Task<IActionResult> Servicos()
        {
            var empresaId = HttpContext.Session.GetInt32("EMPRESA_ID")
                ?? HttpContext.Session.GetInt32("CLIENTE_EMPRESA_ID")
                ?? 0;

            var config = await _context.Configuracoes
                .FirstOrDefaultAsync(c => c.EmpresaId == empresaId);

            return View(config);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}