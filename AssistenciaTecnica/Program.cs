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

// Exibe apenas informações seguras da conexão
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

// Testa conexão e aplica migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    try
    {
        Console.WriteLine("=====================================");
        Console.WriteLine("TESTANDO CONEXÃO COM O POSTGRES...");
        Console.WriteLine("=====================================");

        db.Database.OpenConnection();

        Console.WriteLine("Conexão realizada com sucesso!");

        db.Database.CloseConnection();

        Console.WriteLine("Aplicando Migrations...");

        db.Database.Migrate();

        Console.WriteLine("Banco atualizado com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("=====================================");
        Console.WriteLine("ERRO AO CONECTAR NO POSTGRES");
        Console.WriteLine(ex);
        Console.WriteLine("=====================================");

        throw;
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
    pattern: "{controller=Home}/{action=Landing}/{id?}");

app.Run();