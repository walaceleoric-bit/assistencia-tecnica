using AssistenciaTecnica.Data;
using AssistenciaTecnica.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Obtém a Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("A ConnectionStrings:DefaultConnection não foi encontrada.");
}

// Exibe apenas informações da conexão (sem mostrar a senha)
var builderConnection = new NpgsqlConnectionStringBuilder(connectionString);

Console.WriteLine("=====================================");
Console.WriteLine("CONFIGURAÇÃO DO BANCO");
Console.WriteLine($"Host.....: {builderConnection.Host}");
Console.WriteLine($"Banco....: {builderConnection.Database}");
Console.WriteLine($"Usuário..: {builderConnection.Username}");
Console.WriteLine("=====================================");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CloudinaryService>();

var app = builder.Build();

// ===========================================
// TESTE TEMPORÁRIO
// Não testa conexão e não executa migrations.
// ===========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();