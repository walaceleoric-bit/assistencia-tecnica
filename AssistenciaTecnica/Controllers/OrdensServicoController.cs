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

        private int EmpresaId()
        {
            return HttpContext.Session.GetInt32("EMPRESA_ID") ?? 0;
        }

        private async Task CarregarCombos()
        {
            var empresaId = EmpresaId();

            ViewBag.Clientes = new SelectList(
                await _context.Clientes
                    .Where(c => c.EmpresaId == empresaId)
                    .OrderBy(c => c.Nome)
                    .ToListAsync(),
                "Id",
                "Nome"
            );

            ViewBag.Servicos = new SelectList(
                await _context.Servicos
                    .Where(s => s.EmpresaId == empresaId && s.Ativo)
                    .OrderBy(s => s.Nome)
                    .ToListAsync(),
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

            var webRoot = _webHostEnvironment.WebRootPath;

            if (string.IsNullOrWhiteSpace(webRoot))
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var pastaUploads = Path.Combine(webRoot, "uploads", "ordens");

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

            var empresaId = EmpresaId();

            var ordens = await _context.OrdensServico
                .Include(o => o.Cliente)
                .Include(o => o.Servico)
                .Where(o => o.EmpresaId == empresaId)
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

            var empresaId = EmpresaId();

            if (empresaId == 0)
                return RedirectToAction("Login", "Auth");

            ordem.EmpresaId = empresaId;
            ordem.DataAbertura = DateTime.UtcNow;
            ordem.FotoUrl = await SalvarFotoAsync(foto);

            ordem.Aparelho ??= "";
            ordem.MarcaModelo ??= "";
            ordem.DefeitoRelatado ??= "";
            ordem.ObservacaoTecnica ??= "";
            ordem.Status ??= "Aberta";

            ModelState.Remove("EmpresaId");
            ModelState.Remove("DataAbertura");
            ModelState.Remove("FotoUrl");

            if (!ModelState.IsValid)
            {
                await CarregarCombos();
                TempData["Erro"] = "Verifique os campos obrigatórios.";
                return View(ordem);
            }

            _context.OrdensServico.Add(ordem);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Ordem cadastrada com sucesso!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            var ordem = await _context.OrdensServico
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.EmpresaId == empresaId);

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

            var empresaId = EmpresaId();

            var ordemBanco = await _context.OrdensServico
                .FirstOrDefaultAsync(o =>
                    o.Id == ordem.Id &&
                    o.EmpresaId == empresaId);

            if (ordemBanco == null)
                return RedirectToAction("Index");

            ordem.Aparelho ??= "";
            ordem.MarcaModelo ??= "";
            ordem.DefeitoRelatado ??= "";
            ordem.ObservacaoTecnica ??= "";
            ordem.Status ??= "Aberta";

            ModelState.Remove("EmpresaId");
            ModelState.Remove("DataAbertura");
            ModelState.Remove("FotoUrl");

            if (!ModelState.IsValid)
            {
                await CarregarCombos();
                TempData["Erro"] = "Verifique os campos obrigatórios.";
                return View(ordem);
            }

            ordemBanco.ClienteId = ordem.ClienteId;
            ordemBanco.ServicoId = ordem.ServicoId;
            ordemBanco.Aparelho = ordem.Aparelho;
            ordemBanco.MarcaModelo = ordem.MarcaModelo;
            ordemBanco.DefeitoRelatado = ordem.DefeitoRelatado;
            ordemBanco.ObservacaoTecnica = ordem.ObservacaoTecnica;
            ordemBanco.Status = ordem.Status;
            ordemBanco.Valor = ordem.Valor;

            if (foto != null && foto.Length > 0)
                ordemBanco.FotoUrl = await SalvarFotoAsync(foto);

            if (ordemBanco.Status == "Finalizada" && ordemBanco.DataFinalizacao == null)
                ordemBanco.DataFinalizacao = DateTime.UtcNow;

            if (ordemBanco.Status != "Finalizada")
                ordemBanco.DataFinalizacao = null;

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Ordem atualizada com sucesso!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            var ordem = await _context.OrdensServico
                .Include(o => o.Cliente)
                .Include(o => o.Servico)
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.EmpresaId == empresaId);

            if (ordem == null)
                return RedirectToAction("Index");

            return View(ordem);
        }

        public async Task<IActionResult> Excluir(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            var ordem = await _context.OrdensServico
                .FirstOrDefaultAsync(o =>
                    o.Id == id &&
                    o.EmpresaId == empresaId);

            if (ordem != null)
            {
                _context.OrdensServico.Remove(ordem);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Ordem excluída com sucesso!";
            }

            return RedirectToAction("Index");
        }
    }
}