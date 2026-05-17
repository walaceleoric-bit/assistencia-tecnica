using AssistenciaTecnica.Data;
using AssistenciaTecnica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Controllers
{
    public class ClientesController : Controller
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
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
                return RedirectToAction("Login", "Auth");

            var clientes = await _context.Clientes
                .OrderByDescending(c => c.DataCadastro)
                .ToListAsync();

            return View(clientes);
        }

        public IActionResult Criar()
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            return View(new Cliente());
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Cliente cliente)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(cliente);

            cliente.DataCadastro = DateTime.Now;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return RedirectToAction("Index");

            return View(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Cliente cliente)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(cliente);

            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return RedirectToAction("Index");

            return View(cliente);
        }

        public async Task<IActionResult> Excluir(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}