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
            return RedirectToAction("Login", "Auth");
        }

        public async Task<IActionResult> Index()
        {
            if (!DonoLogado())
                return RedirectToAction("Login", "Auth");

            var empresas = await _context.Empresas
                .OrderByDescending(e => e.DataCadastro)
                .ToListAsync();

            return View(empresas);
        }

        public IActionResult Criar()
        {
            if (!DonoLogado())
                return RedirectToAction("Login", "Auth");

            return View(new Empresa());
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Empresa empresa)
        {
            if (!DonoLogado())
                return RedirectToAction("Login", "Auth");

            empresa.NomeEmpresa ??= "";
            empresa.Usuario ??= "";
            empresa.Senha ??= "";
            empresa.WhatsApp ??= "";
            empresa.Ativo = true;
            empresa.DataCadastro = DateTime.UtcNow;

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (!DonoLogado())
                return RedirectToAction("Login", "Auth");

            var empresa = await _context.Empresas.FindAsync(id);

            if (empresa == null)
                return RedirectToAction("Index");

            return View(empresa);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Empresa empresa)
        {
            if (!DonoLogado())
                return RedirectToAction("Login", "Auth");

            var empresaBanco = await _context.Empresas
                .FirstOrDefaultAsync(e => e.Id == empresa.Id);

            if (empresaBanco == null)
                return RedirectToAction("Index");

            empresaBanco.NomeEmpresa = empresa.NomeEmpresa ?? "";
            empresaBanco.Usuario = empresa.Usuario ?? "";
            empresaBanco.Senha = empresa.Senha ?? "";
            empresaBanco.WhatsApp = empresa.WhatsApp ?? "";
            empresaBanco.Ativo = empresa.Ativo;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AlternarStatus(int id)
        {
            if (!DonoLogado())
                return RedirectToAction("Login", "Auth");

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
            HttpContext.Session.Clear();
            return RedirectToAction("Landing", "Home");
        }
    }
}