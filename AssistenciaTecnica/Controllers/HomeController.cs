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

        public IActionResult Landing()
        {
            HttpContext.Session.Clear();
            return View();
        }

        private int EmpresaId()
        {
            var empresaAdm = HttpContext.Session.GetInt32("EMPRESA_ID") ?? 0;

            if (empresaAdm > 0)
                return empresaAdm;

            var empresaCliente = HttpContext.Session.GetInt32("CLIENTE_EMPRESA_ID") ?? 0;

            if (empresaCliente > 0)
                return empresaCliente;

            return 0;
        }

        private async Task<Configuracao> ObterConfiguracaoDaEmpresa()
        {
            var empresaId = EmpresaId();

            if (empresaId == 0)
            {
                return new Configuracao
                {
                    NomeEmpresa = "Empresa",
                    SubtituloEmpresa = "Assistência Técnica"
                };
            }

            var config = await _context.Configuracoes
                .FirstOrDefaultAsync(c => c.EmpresaId == empresaId);

            if (config == null)
            {
                config = new Configuracao
                {
                    EmpresaId = empresaId,
                    NomeEmpresa = "Empresa",
                    SubtituloEmpresa = "Assistência Técnica",
                    SenhaAdm = "123456"
                };

                _context.Configuracoes.Add(config);
                await _context.SaveChangesAsync();
            }

            return config;
        }

        public async Task<IActionResult> Index()
        {
            var config = await ObterConfiguracaoDaEmpresa();
            return View(config);
        }

        public IActionResult Servicos()
        {
            if (HttpContext.Session.GetString("CLIENTE_LOGADO") == "SIM")
                return RedirectToAction("Index", "AreaCliente");

            return View();
        }

        public async Task<IActionResult> Contato()
        {
            var config = await ObterConfiguracaoDaEmpresa();
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