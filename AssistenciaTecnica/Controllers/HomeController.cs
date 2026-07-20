using Microsoft.AspNetCore.Mvc;

namespace AssistenciaTecnica.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var admLogado = HttpContext.Session.GetString("ADM_LOGADO") == "SIM";
            var clienteLogado = HttpContext.Session.GetString("CLIENTE_LOGADO") == "SIM";

            // Se ninguém estiver autenticado na sessão, redireciona para a tela de Login
            if (!admLogado && !clienteLogado)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
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