using Cuentas.Dominio.Entidades;
using Cuentas.Infraestructura.Persistencia;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cuentas.Integracion.Pruebas;

public sealed class FabricaCuentasPruebas : WebApplicationFactory<Program>
{
    public static readonly Guid ClienteActivoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ClienteInactivoId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private readonly SqliteConnection conexion = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        conexion.Open();
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(servicios =>
        {
            servicios.RemoveAll<DbContextOptions<ContextoCuentas>>();
            servicios.RemoveAll<IDbContextOptionsConfiguration<ContextoCuentas>>();
            servicios.AddDbContext<ContextoCuentas>(opciones => opciones.UseSqlite(conexion));

            ServiceProvider proveedorServicios = servicios.BuildServiceProvider();
            using IServiceScope alcance = proveedorServicios.CreateScope();
            ContextoCuentas contextoCuentas = alcance.ServiceProvider.GetRequiredService<ContextoCuentas>();
            contextoCuentas.Database.EnsureCreated();
            contextoCuentas.ClientesIntegracion.AddRange(
                new ClienteIntegracion(ClienteActivoId, "Cliente activo", true, DateTime.UtcNow),
                new ClienteIntegracion(ClienteInactivoId, "Cliente inactivo", false, DateTime.UtcNow));
            contextoCuentas.SaveChanges();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        conexion.Dispose();
    }
}
