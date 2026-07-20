using AssistenciaTecnica.Data;
using AssistenciaTecnica.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Obtém a Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionStrings__DefaultConnection"]
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("A ConnectionStrings:DefaultConnection não foi encontrada.");
}

// Exibe informações da conexão nos logs para conferência
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

// =========================================================
// APLICA MIGRATIONS AUTOMATICAMENTE AO INICIAR
// Cria o banco de dados e as tabelas caso ainda não existam no Render
// =========================================================
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
        Console.WriteLine("Migrations aplicadas com sucesso no PostgreSQL!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao aplicar migrations: {ex.Message}");
    }
}

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