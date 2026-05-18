using AssistenciaTecnica.Models;
using Microsoft.EntityFrameworkCore;

namespace AssistenciaTecnica.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Empresa> Empresas { get; set; }

        public DbSet<Configuracao> Configuracoes { get; set; }

        public DbSet<Servico> Servicos { get; set; }

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<OrdemServico> OrdensServico { get; set; }
    }
}