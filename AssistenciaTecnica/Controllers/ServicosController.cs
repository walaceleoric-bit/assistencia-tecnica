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

        private int EmpresaId()
        {
            return HttpContext.Session.GetInt32("EMPRESA_ID") ?? 0;
        }

        public async Task<IActionResult> Index()
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            var servicos = await _context.Servicos
                .Where(s => s.EmpresaId == empresaId)
                .OrderBy(s => s.Nome)
                .ToListAsync();

            return View(servicos);
        }

        public IActionResult Criar()
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            return View(new Servico());
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Servico servico)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            if (empresaId == 0)
                return RedirectToAction("Login", "Auth");

            servico.EmpresaId = empresaId;
            servico.Nome ??= "";
            servico.Descricao ??= "";

            ModelState.Remove("EmpresaId");

            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Informe pelo menos o nome do serviço.";
                return View(servico);
            }

            _context.Servicos.Add(servico);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Serviço cadastrado com sucesso!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            var servico = await _context.Servicos
                .FirstOrDefaultAsync(s =>
                    s.Id == id &&
                    s.EmpresaId == empresaId);

            if (servico == null)
                return RedirectToAction("Index");

            return View(servico);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Servico servico)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            var servicoBanco = await _context.Servicos
                .FirstOrDefaultAsync(s =>
                    s.Id == servico.Id &&
                    s.EmpresaId == empresaId);

            if (servicoBanco == null)
                return RedirectToAction("Index");

            servico.Nome ??= "";
            servico.Descricao ??= "";

            ModelState.Remove("EmpresaId");

            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Informe pelo menos o nome do serviço.";
                return View(servico);
            }

            servicoBanco.Nome = servico.Nome;
            servicoBanco.Descricao = servico.Descricao;
            servicoBanco.Ativo = servico.Ativo;

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Serviço atualizado com sucesso!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Excluir(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            var servico = await _context.Servicos
                .FirstOrDefaultAsync(s =>
                    s.Id == id &&
                    s.EmpresaId == empresaId);

            if (servico != null)
            {
                _context.Servicos.Remove(servico);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Serviço excluído com sucesso!";
            }

            return RedirectToAction("Index");
        }
    }
}