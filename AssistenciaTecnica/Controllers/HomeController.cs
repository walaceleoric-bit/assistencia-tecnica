using AssistenciaTecnica.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AssistenciaTecnica.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Landing()
        {
            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            return View(new Configuracao
            {
                NomeEmpresa = "Teste",
                SubtituloEmpresa = "Sistema funcionando sem banco"
            });
        }

        public IActionResult Servicos()
        {
            return View();
        }

        public IActionResult Contato()
        {
            return View(new Configuracao
            {
                NomeEmpresa = "Teste",
                SubtituloEmpresa = "Sistema funcionando sem banco"
            });
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