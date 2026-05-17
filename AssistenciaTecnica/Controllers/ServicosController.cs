using AssistenciaTecnica.Data;
using AssistenciaTecnica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Controllers
{
    public class ServicosController : Controller
    {
        private readonly AppDbContext _context;

        public ServicosController(AppDbContext context)
        {
            _context = context;
        }

        private bool AdminLogado()
        {
            return HttpContext.Session.GetString("ADM_LOGADO") == "SIM";
        }

        public async Task<IActionResult> Index()
        {
            if (!AdminLogado())
            {
                return RedirectToAction("Login", "Auth");
            }

            var servicos = await _context.Servicos
                .OrderBy(s => s.Nome)
                .ToListAsync();

            return View(servicos);
        }

        public IActionResult Criar()
        {
            if (!AdminLogado())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(new Servico());
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Servico servico)
        {
            if (!AdminLogado())
            {
                return RedirectToAction("Login", "Auth");
            }

            _context.Servicos.Add(servico);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (!AdminLogado())
            {
                return RedirectToAction("Login", "Auth");
            }

            var servico = await _context.Servicos.FindAsync(id);

            if (servico == null)
            {
                return RedirectToAction("Index");
            }

            return View(servico);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Servico servico)
        {
            if (!AdminLogado())
            {
                return RedirectToAction("Login", "Auth");
            }

            _context.Servicos.Update(servico);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Excluir(int id)
        {
            if (!AdminLogado())
            {
                return RedirectToAction("Login", "Auth");
            }

            var servico = await _context.Servicos.FindAsync(id);

            if (servico != null)
            {
                _context.Servicos.Remove(servico);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}