using AssistenciaTecnica.Data;
using AssistenciaTecnica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AssistenciaTecnica.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // Página pública principal do sistema SaaS
        public IActionResult Landing()
        {
            return View();
        }

        // Home personalizada da empresa
        public async Task<IActionResult> Index()
        {
            var config = await _context.Configuracoes.FirstOrDefaultAsync();

            if (config == null)
            {
                config = new Configuracao();
                _context.Configuracoes.Add(config);
                await _context.SaveChangesAsync();
            }

            return View(config);
        }

        public IActionResult Servicos()
        {
            return View();
        }

        public async Task<IActionResult> Contato()
        {
            var config = await _context.Configuracoes.FirstOrDefaultAsync();

            if (config == null)
            {
                config = new Configuracao();
                _context.Configuracoes.Add(config);
                await _context.SaveChangesAsync();
            }

            return View(config);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}