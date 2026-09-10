using Clientes.Infraestructura.Persistencia;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Clientes.Integracion.Pruebas;

public sealed class FabricaClientesPruebas : WebApplicationFactory<Program>
{
    private readonly SqliteConnection conexion = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        conexion.Open();
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(servicios =>
        {
            servicios.RemoveAll<DbContextOptions<ContextoClientes>>();
            servicios.RemoveAll<IDbContextOptionsConfiguration<ContextoClientes>>();
            servicios.AddDbContext<ContextoClientes>(opciones => opciones.UseSqlite(conexion));

            ServiceProvider proveedorServicios = servicios.BuildServiceProvider();
            using IServiceScope alcance = proveedorServicios.CreateScope();
            ContextoClientes contextoClientes = alcance.ServiceProvider.GetRequiredService<ContextoClientes>();
            contextoClientes.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        conexion.Dispose();
    }
}
