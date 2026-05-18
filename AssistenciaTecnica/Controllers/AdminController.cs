using AssistenciaTecnica.Data;
using AssistenciaTecnica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(
            AppDbContext context,
            IWebHostEnvironment webHostEnvironment)
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

        private async Task<string> SalvarImagemAsync(IFormFile? arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return "";

            var webRoot = _webHostEnvironment.WebRootPath;

            if (string.IsNullOrEmpty(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var pasta = Path.Combine(webRoot, "uploads", "site");

            if (!Directory.Exists(pasta))
            {
                Directory.CreateDirectory(pasta);
            }

            var extensao = Path.GetExtension(arquivo.FileName);
            var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
            var caminhoCompleto = Path.Combine(pasta, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return $"/uploads/site/{nomeArquivo}";
        }

        private async Task<Configuracao> ObterConfiguracao()
        {
            var empresaId = EmpresaId();

            if (empresaId == 0)
                throw new Exception("Empresa não identificada na sessão.");

            var config = await _context.Configuracoes
                .FirstOrDefaultAsync(c => c.EmpresaId == empresaId);

            if (config == null)
            {
                config = new Configuracao
                {
                    EmpresaId = empresaId,
                    NomeEmpresa = "Milton Cardoso",
                    SubtituloEmpresa = "Assistência Técnica",
                    WhatsApp = "",
                    CidadesAtendidas = "Serra e Vitória",
                    LogoUrl = "",
                    TituloPrincipal = "Conserto de Eletrodomésticos",
                    TextoPrincipal = "Máquina de lavar, TV, micro-ondas e muito mais. Atendimento rápido, profissional e com garantia.",
                    Destaque1Titulo = "Máquinas de lavar",
                    Destaque1Texto = "Conserto, manutenção e revisão de máquinas de lavar.",
                    Destaque1ImagemUrl = "",
                    Destaque2Titulo = "Televisores e Micro-ondas",
                    Destaque2Texto = "Manutenção em televisores, micro-ondas e outros eletrodomésticos.",
                    Destaque2ImagemUrl = "",
                    SenhaAdm = "123456"
                };

                _context.Configuracoes.Add(config);
                await _context.SaveChangesAsync();
            }

            bool alterou = false;

            if (string.IsNullOrWhiteSpace(config.SenhaAdm))
            {
                config.SenhaAdm = "123456";
                alterou = true;
            }

            if (string.IsNullOrWhiteSpace(config.NomeEmpresa))
            {
                config.NomeEmpresa = "Milton Cardoso";
                alterou = true;
            }

            if (string.IsNullOrWhiteSpace(config.SubtituloEmpresa))
            {
                config.SubtituloEmpresa = "Assistência Técnica";
                alterou = true;
            }

            if (config.WhatsApp == null)
            {
                config.WhatsApp = "";
                alterou = true;
            }

            if (config.CidadesAtendidas == null)
            {
                config.CidadesAtendidas = "";
                alterou = true;
            }

            if (config.LogoUrl == null)
            {
                config.LogoUrl = "";
                alterou = true;
            }

            if (string.IsNullOrWhiteSpace(config.TituloPrincipal))
            {
                config.TituloPrincipal = "Conserto de Eletrodomésticos";
                alterou = true;
            }

            if (string.IsNullOrWhiteSpace(config.TextoPrincipal))
            {
                config.TextoPrincipal = "Máquina de lavar, TV, micro-ondas e muito mais. Atendimento rápido, profissional e com garantia.";
                alterou = true;
            }

            if (string.IsNullOrWhiteSpace(config.Destaque1Titulo) || config.Destaque1Titulo.StartsWith("/"))
            {
                config.Destaque1Titulo = "Máquinas de lavar";
                alterou = true;
            }

            if (string.IsNullOrWhiteSpace(config.Destaque1Texto) || config.Destaque1Texto.StartsWith("/"))
            {
                config.Destaque1Texto = "Conserto, manutenção e revisão de máquinas de lavar.";
                alterou = true;
            }

            if (config.Destaque1ImagemUrl == null)
            {
                config.Destaque1ImagemUrl = "";
                alterou = true;
            }

            if (string.IsNullOrWhiteSpace(config.Destaque2Titulo) || config.Destaque2Titulo.StartsWith("/"))
            {
                config.Destaque2Titulo = "Televisores e Micro-ondas";
                alterou = true;
            }

            if (string.IsNullOrWhiteSpace(config.Destaque2Texto) || config.Destaque2Texto.StartsWith("/"))
            {
                config.Destaque2Texto = "Manutenção em televisores, micro-ondas e outros eletrodomésticos.";
                alterou = true;
            }

            if (config.Destaque2ImagemUrl == null)
            {
                config.Destaque2ImagemUrl = "";
                alterou = true;
            }

            if (alterou)
            {
                _context.Configuracoes.Update(config);
                await _context.SaveChangesAsync();
            }

            return config;
        }

        public IActionResult Index()
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            return View();
        }

        public async Task<IActionResult> Configuracao()
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var config = await ObterConfiguracao();

            return View(config);
        }

        [HttpPost]
        public async Task<IActionResult> Configuracao(
            Configuracao config,
            IFormFile? logo,
            IFormFile? destaque1,
            IFormFile? destaque2)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            if (empresaId == 0)
                return RedirectToAction("Login", "Auth");

            var configBanco = await _context.Configuracoes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.EmpresaId == empresaId);

            if (configBanco == null)
                return RedirectToAction("Configuracao");

            var novaLogo = await SalvarImagemAsync(logo);
            var novaDestaque1 = await SalvarImagemAsync(destaque1);
            var novaDestaque2 = await SalvarImagemAsync(destaque2);

            config.Id = configBanco.Id;
            config.EmpresaId = empresaId;

            config.LogoUrl = string.IsNullOrWhiteSpace(novaLogo)
                ? configBanco.LogoUrl
                : novaLogo;

            config.Destaque1ImagemUrl = string.IsNullOrWhiteSpace(novaDestaque1)
                ? configBanco.Destaque1ImagemUrl
                : novaDestaque1;

            config.Destaque2ImagemUrl = string.IsNullOrWhiteSpace(novaDestaque2)
                ? configBanco.Destaque2ImagemUrl
                : novaDestaque2;

            if (string.IsNullOrWhiteSpace(config.SenhaAdm))
            {
                config.SenhaAdm = configBanco.SenhaAdm;

                if (string.IsNullOrWhiteSpace(config.SenhaAdm))
                    config.SenhaAdm = "123456";
            }

            config.NomeEmpresa ??= "";
            config.SubtituloEmpresa ??= "";
            config.WhatsApp ??= "";
            config.CidadesAtendidas ??= "";
            config.LogoUrl ??= "";
            config.TituloPrincipal ??= "";
            config.TextoPrincipal ??= "";
            config.Destaque1Titulo ??= "";
            config.Destaque1Texto ??= "";
            config.Destaque1ImagemUrl ??= "";
            config.Destaque2Titulo ??= "";
            config.Destaque2Texto ??= "";
            config.Destaque2ImagemUrl ??= "";
            config.SenhaAdm ??= "123456";

            _context.Configuracoes.Update(config);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Configurações salvas com sucesso!";
            return RedirectToAction("Configuracao");
        }
    }
}