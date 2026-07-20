using AssistenciaTecnica.Data;
using AssistenciaTecnica.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// 1. Obtém a Connection String (tenta carregar de todas as formas que o Render/Linux disponibiliza)
var rawConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? builder.Configuration["ConnectionStrings__DefaultConnection"]
    ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
    ?? builder.Configuration["DATABASE_URL"];

if (string.IsNullOrWhiteSpace(rawConnectionString))
{
    throw new Exception("Nenhuma Connection String de banco de dados foi encontrada nas configurações.");
}

// 2. Converte a conexão se ela vier no formato URL (postgres://...) do Render
string connectionString;

if (rawConnectionString.StartsWith("postgres://") || rawConnectionString.StartsWith("postgresql://"))
{
    var databaseUri = new Uri(rawConnectionString);
    var userInfo = databaseUri.UserInfo.Split(':');

    var npgsqlBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = databaseUri.Host,
        Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
        Username = userInfo[0],
        Password = userInfo.Length > 1 ? userInfo[1] : "",
        Database = databaseUri.LocalPath.TrimStart('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    };
    connectionString = npgsqlBuilder.ToString();
}
else
{
    // Se for formato padrão Key-Value, garante que SSL esteja configurado
    var npgsqlBuilder = new NpgsqlConnectionStringBuilder(rawConnectionString)
    {
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    };
    connectionString = npgsqlBuilder.ToString();
}

// Log de diagnóstico mascarado para conferência nos Logs do Render
var logBuilder = new NpgsqlConnectionStringBuilder(connectionString);
Console.WriteLine("=====================================");
Console.WriteLine("CONECTANDO AO BANCO DE DADOS");
Console.WriteLine($"Host.....: {logBuilder.Host}");
Console.WriteLine($"Porta....: {logBuilder.Port}");
Console.WriteLine($"Banco....: {logBuilder.Database}");
Console.WriteLine($"Usuário..: {logBuilder.Username}");
Console.WriteLine("=====================================");

// 3. Configura o DbContext com Resiliência de Conexão
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    }));

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

// 4. Aplica Migrations automaticamente ao subir a aplicação
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
        Console.WriteLine("Migrations aplicadas/verificadas com sucesso no PostgreSQL!");
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

// ROTA PADRÃO ALTERADA PARA TELA DE LOGIN
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();