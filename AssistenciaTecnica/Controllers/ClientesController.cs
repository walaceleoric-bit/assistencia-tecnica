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

        private int EmpresaId()
        {
            return HttpContext.Session.GetInt32("EMPRESA_ID") ?? 0;
        }

        public async Task<IActionResult> Index()
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            var clientes = await _context.Clientes
                .Where(c => c.EmpresaId == empresaId)
                .OrderByDescending(c => c.DataCadastro)
                .ToListAsync();

            return View(clientes);
        }

        public async Task<IActionResult> Detalhes(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.EmpresaId == empresaId);

            if (cliente == null)
                return RedirectToAction(nameof(Index));

            return View(cliente);
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

            var empresaId = EmpresaId();

            if (empresaId == 0)
                return RedirectToAction("Login", "Auth");

            cliente.EmpresaId = empresaId;
            cliente.DataCadastro = DateTime.UtcNow;

            cliente.Nome ??= "";
            cliente.Cpf ??= "";
            cliente.UsuarioCliente ??= "";
            cliente.SenhaCliente ??= "";
            cliente.Telefone ??= "";
            cliente.Email ??= "";
            cliente.Endereco ??= "";
            cliente.Cidade ??= "";
            cliente.Observacao ??= "";

            ModelState.Remove("EmpresaId");
            ModelState.Remove("DataCadastro");

            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Verifique os campos obrigatórios.";
                return View(cliente);
            }

            var usuarioExiste = await _context.Clientes.AnyAsync(c =>
                c.EmpresaId == empresaId &&
                c.UsuarioCliente == cliente.UsuarioCliente &&
                cliente.UsuarioCliente != "");

            if (usuarioExiste)
            {
                TempData["Erro"] = "Este usuário de acesso já está sendo usado por outro cliente.";
                return View(cliente);
            }

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Cliente cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.EmpresaId == empresaId);

            if (cliente == null)
                return RedirectToAction(nameof(Index));

            return View(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Cliente cliente)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            if (empresaId == 0)
                return RedirectToAction("Login", "Auth");

            var clienteBanco = await _context.Clientes
                .FirstOrDefaultAsync(c =>
                    c.Id == cliente.Id &&
                    c.EmpresaId == empresaId);

            if (clienteBanco == null)
                return RedirectToAction(nameof(Index));

            cliente.Nome ??= "";
            cliente.Cpf ??= "";
            cliente.UsuarioCliente ??= "";
            cliente.SenhaCliente ??= "";
            cliente.Telefone ??= "";
            cliente.Email ??= "";
            cliente.Endereco ??= "";
            cliente.Cidade ??= "";
            cliente.Observacao ??= "";

            ModelState.Remove("EmpresaId");
            ModelState.Remove("DataCadastro");

            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Verifique os campos obrigatórios.";
                return View(cliente);
            }

            var usuarioExiste = await _context.Clientes.AnyAsync(c =>
                c.EmpresaId == empresaId &&
                c.Id != cliente.Id &&
                c.UsuarioCliente == cliente.UsuarioCliente &&
                cliente.UsuarioCliente != "");

            if (usuarioExiste)
            {
                TempData["Erro"] = "Este usuário de acesso já está sendo usado por outro cliente.";
                return View(cliente);
            }

            clienteBanco.Nome = cliente.Nome;
            clienteBanco.Cpf = cliente.Cpf;
            clienteBanco.UsuarioCliente = cliente.UsuarioCliente;
            clienteBanco.SenhaCliente = cliente.SenhaCliente;
            clienteBanco.Telefone = cliente.Telefone;
            clienteBanco.Email = cliente.Email;
            clienteBanco.Endereco = cliente.Endereco;
            clienteBanco.Cidade = cliente.Cidade;
            clienteBanco.Observacao = cliente.Observacao;

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Cliente atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Excluir(int id)
        {
            if (!AdminLogado())
                return RedirectToAction("Login", "Auth");

            var empresaId = EmpresaId();

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.EmpresaId == empresaId);

            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                TempData["Sucesso"] = "Cliente excluído com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}