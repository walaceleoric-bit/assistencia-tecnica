using AssistenciaTecnica.Data;
using AssistenciaTecnica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Controllers
{
    public class OrdensServicoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public OrdensServicoController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private bool AdminLogado()
        {
            return HttpContext.Session.GetString("ADM_LOGADO") == "SIM";
        }

        private async Task CarregarCombos()
        {
            ViewBag.Clientes = new SelectList(
                await _context.Clientes.OrderBy(c => c.Nome).ToListAsync(),
                "Id",
                "Nome"
            );

            ViewBag.Servicos = new SelectList(
                await _context.Servicos.Where(s => s.Ativo).OrderBy(s => s.Nome).ToListAsync(),
                "Id",
                "Nome"
            );

            ViewBag.Status = new SelectList(new List<string>
            {
                "Aberta",
                "Em análise",
                "Aguardando orçamento",
                "Orçamento aprovado",
                "Em manutenção",
                "Finalizada",
                "Cancelada"
            });
        }

        private async Task<string> SalvarFotoAsync(IFormFile? foto)
        {
            if (foto == null || foto.Length == 0)
                return "";

            var pastaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "ordens");

            if (!Directory.Exists(pastaUploads))
                Directory.CreateDirectory(pastaUploads);

            var extensao = Path.GetExtension(foto.FileName);
            var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
            var caminhoCompleto = Path.Combine(pastaUploads, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await foto.CopyToAsync(stream);
            }

            return $"/uploads/ordens/{nomeArquivo}";
        }

        public async Task<IActionResult> Index()
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var ordens = await _context.OrdensServico
                .Include(o => o.Cliente)
                .Include(o => o.Servico)
                .OrderByDescending(o => o.DataAbertura)
                .ToListAsync();

            return View(ordens);
        }

        public async Task<IActionResult> Criar()
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            await CarregarCombos();
            return View(new OrdemServico());
        }

        [HttpPost]
        public async Task<IActionResult> Criar(OrdemServico ordem, IFormFile? foto)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            ordem.DataAbertura = DateTime.Now;
            ordem.FotoUrl = await SalvarFotoAsync(foto);

            _context.OrdensServico.Add(ordem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var ordem = await _context.OrdensServico.FindAsync(id);

            if (ordem == null)
                return RedirectToAction("Index");

            await CarregarCombos();
            return View(ordem);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(OrdemServico ordem, IFormFile? foto)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var ordemBanco = await _context.OrdensServico.AsNoTracking().FirstOrDefaultAsync(o => o.Id == ordem.Id);

            if (ordemBanco == null)
                return RedirectToAction("Index");

            if (foto != null && foto.Length > 0)
            {
                ordem.FotoUrl = await SalvarFotoAsync(foto);
            }
            else
            {
                ordem.FotoUrl = ordemBanco.FotoUrl;
            }

            if (ordem.Status == "Finalizada" && ordem.DataFinalizacao == null)
                ordem.DataFinalizacao = DateTime.Now;

            _context.OrdensServico.Update(ordem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var ordem = await _context.OrdensServico
                .Include(o => o.Cliente)
                .Include(o => o.Servico)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ordem == null)
                return RedirectToAction("Index");

            return View(ordem);
        }

        public async Task<IActionResult> Excluir(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var ordem = await _context.OrdensServico.FindAsync(id);

            if (ordem != null)
            {
                _context.OrdensServico.Remove(ordem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}