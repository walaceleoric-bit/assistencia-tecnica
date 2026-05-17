using AssistenciaTecnica.Data;
using AssistenciaTecnica.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Controllers
{
    public class RelatoriosController : Controller
    {
        private readonly AppDbContext _context;

        public RelatoriosController(AppDbContext context)
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

            var ordens = await _context.OrdensServico
                .Include(o => o.Servico)
                .ToListAsync();

            var finalizadas = ordens
                .Where(o => o.Status == "Finalizada")
                .ToList();

            var abertas = ordens
                .Where(o => o.Status != "Finalizada" && o.Status != "Cancelada")
                .ToList();

            var model = new RelatorioViewModel();

            model.TotalClientes = await _context.Clientes.CountAsync();
            model.TotalServicos = await _context.Servicos.CountAsync();
            model.TotalOrdens = ordens.Count;

            model.OrdensAbertas = abertas.Count;
            model.OrdensFinalizadas = finalizadas.Count;
            model.OrdensCanceladas = ordens.Count(o => o.Status == "Cancelada");

            model.FaturamentoFinalizado = finalizadas.Sum(o => o.Valor);
            model.FaturamentoEmAberto = abertas.Sum(o => o.Valor);

            model.TicketMedio = finalizadas.Any()
                ? finalizadas.Average(o => o.Valor)
                : 0;

            var porStatus = ordens
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            model.StatusLabels = porStatus.Select(x => x.Status).ToList();
            model.StatusValores = porStatus.Select(x => x.Total).ToList();

            var porServico = ordens
                .Where(o => o.Servico != null)
                .GroupBy(o => o.Servico!.Nome)
                .Select(g => new
                {
                    Servico = g.Key,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .Take(6)
                .ToList();

            model.ServicosLabels = porServico.Select(x => x.Servico).ToList();
            model.ServicosValores = porServico.Select(x => x.Total).ToList();

            var faturamentoMes = finalizadas
                .GroupBy(o => new
                {
                    o.DataAbertura.Year,
                    o.DataAbertura.Month
                })
                .Select(g => new
                {
                    Mes = $"{g.Key.Month:00}/{g.Key.Year}",
                    Total = g.Sum(x => x.Valor)
                })
                .OrderBy(x => x.Mes)
                .ToList();

            model.MesesLabels = faturamentoMes.Select(x => x.Mes).ToList();
            model.FaturamentoMesValores = faturamentoMes.Select(x => x.Total).ToList();

            return View(model);
        }
    }
}