using AssistenciaTecnica.Data;
using AssistenciaTecnica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Controllers
{
    public class DonoController : Controller
    {
        private readonly AppDbContext _context;

        public DonoController(AppDbContext context)
        {
            _context = context;
        }

        private bool DonoLogado()
        {
            return HttpContext.Session.GetString("DONO_LOGADO") == "SIM";
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string usuario, string senha)
        {
            if (usuario == "wlc" && senha == "123456")
            {
                HttpContext.Session.SetString("DONO_LOGADO", "SIM");
                return RedirectToAction("Index");
            }

            TempData["Erro"] = "Usuário ou senha inválidos.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Index()
        {
            if (!DonoLogado())
                return RedirectToAction("Login");

            var empresas = await _context.Empresas
                .OrderByDescending(e => e.DataCadastro)
                .ToListAsync();

            return View(empresas);
        }

        public IActionResult Criar()
        {
            if (!DonoLogado())
                return RedirectToAction("Login");

            return View(new Empresa());
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Empresa empresa)
        {
            if (!DonoLogado())
                return RedirectToAction("Login");

            empresa.DataCadastro = DateTime.UtcNow;
            empresa.Ativo = true;

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (!DonoLogado())
                return RedirectToAction("Login");

            var empresa = await _context.Empresas.FindAsync(id);

            if (empresa == null)
                return RedirectToAction("Index");

            return View(empresa);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Empresa empresa)
        {
            if (!DonoLogado())
                return RedirectToAction("Login");

            _context.Empresas.Update(empresa);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AlternarStatus(int id)
        {
            if (!DonoLogado())
                return RedirectToAction("Login");

            var empresa = await _context.Empresas.FindAsync(id);

            if (empresa != null)
            {
                empresa.Ativo = !empresa.Ativo;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Sair()
        {
            HttpContext.Session.Remove("DONO_LOGADO");
            return RedirectToAction("Landing", "Home");
        }
    }
}