using AssistenciaTecnica.Data;
using AssistenciaTecnica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var config = await ObterConfiguracaoAsync();

            return View(config);
        }

        public async Task<IActionResult> Contato()
        {
            var config = await ObterConfiguracaoAsync();

            return View(config);
        }

        public async Task<IActionResult> Servicos()
        {
            var config = await ObterConfiguracaoAsync();

            return View(config);
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

        private async Task<Configuracao> ObterConfiguracaoAsync()
        {
            try
            {
                var empresaId = HttpContext.Session.GetInt32("EMPRESA_ID")
                    ?? HttpContext.Session.GetInt32("CLIENTE_EMPRESA_ID");

                // Se existe uma empresa na sessão, tenta carregar
                // primeiro a configuração dessa empresa.
                if (empresaId.HasValue && empresaId.Value > 0)
                {
                    var configEmpresa = await _context.Configuracoes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.EmpresaId == empresaId.Value);

                    if (configEmpresa != null)
                    {
                        return configEmpresa;
                    }
                }

                // Página pública:
                // utiliza a primeira configuração cadastrada.
                var configPublica = await _context.Configuracoes
                    .AsNoTracking()
                    .OrderBy(c => c.Id)
                    .FirstOrDefaultAsync();

                if (configPublica != null)
                {
                    return configPublica;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Erro ao carregar configuração da página pública: {ex.Message}"
                );
            }

            // Se o banco estiver temporariamente indisponível,
            // a página continua abrindo em vez de gerar erro 500.
            return new Configuracao
            {
                NomeEmpresa = "Milton Cardoso",
                SubtituloEmpresa = "Assistência Técnica",
                CidadesAtendidas = "Serra e Vitória",
                TituloPrincipal = "Conserto de Eletrodomésticos",
                TextoPrincipal = "Assistência técnica com atendimento rápido, profissional e com garantia."
            };
        }
    }
}