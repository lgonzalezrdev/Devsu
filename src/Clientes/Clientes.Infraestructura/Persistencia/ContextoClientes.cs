using Clientes.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infraestructura.Persistencia;

public sealed class ContextoClientes(DbContextOptions<ContextoClientes> opciones) : DbContext(opciones)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();

    public DbSet<CuentaReporte> CuentasReporte => Set<CuentaReporte>();

    public DbSet<MovimientoReporte> MovimientosReporte => Set<MovimientoReporte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Persona>(entidad =>
        {
            entidad.ToTable("Personas", "clientes");
            entidad.HasKey(persona => persona.PersonaId);
            entidad.Property(persona => persona.PersonaId).ValueGeneratedNever();
            entidad.Property(persona => persona.Nombre).HasMaxLength(150).IsRequired();
            entidad.Property(persona => persona.Genero).HasConversion<string>().HasMaxLength(30).IsRequired();
            entidad.Property(persona => persona.Edad).IsRequired();
            entidad.Property(persona => persona.Identificacion).HasMaxLength(30).IsRequired();
            entidad.HasIndex(persona => persona.Identificacion).IsUnique();
            entidad.Property(persona => persona.Direccion).HasMaxLength(250);
            entidad.Property(persona => persona.Telefono).HasMaxLength(30);
        });

        modelBuilder.Entity<Cliente>(entidad =>
        {
            entidad.ToTable("Clientes", "clientes");
            entidad.Ignore(cliente => cliente.ClienteId);
            entidad.Property(cliente => cliente.ContrasenaHash).HasMaxLength(500).IsRequired();
            entidad.Property(cliente => cliente.Estado).IsRequired();
        });

        modelBuilder.Entity<CuentaReporte>(entidad =>
        {
            entidad.ToTable("CuentasReporte", "clientes");
            entidad.HasKey(cuenta => cuenta.CuentaId);
            entidad.Property(cuenta => cuenta.CuentaId).ValueGeneratedNever();
            entidad.Property(cuenta => cuenta.ClienteId).IsRequired();
            entidad.Property(cuenta => cuenta.NumeroCuenta).HasMaxLength(20).IsRequired();
            entidad.Property(cuenta => cuenta.TipoCuenta).HasMaxLength(20).IsRequired();
            entidad.Property(cuenta => cuenta.SaldoInicial).HasPrecision(18, 2).IsRequired();
            entidad.Property(cuenta => cuenta.SaldoDisponible).HasPrecision(18, 2).IsRequired();
            entidad.Property(cuenta => cuenta.Estado).IsRequired();
            entidad.Property(cuenta => cuenta.ActualizadoEn).HasColumnType("datetime2(0)").IsRequired();
            entidad.HasIndex(cuenta => cuenta.ClienteId);
        });

        modelBuilder.Entity<MovimientoReporte>(entidad =>
        {
            entidad.ToTable("MovimientosReporte", "clientes");
            entidad.HasKey(movimiento => movimiento.MovimientoId);
            entidad.Property(movimiento => movimiento.MovimientoId).ValueGeneratedNever();
            entidad.Property(movimiento => movimiento.Fecha).HasColumnType("datetime2(0)").IsRequired();
            entidad.Property(movimiento => movimiento.TipoMovimiento).HasMaxLength(20).IsRequired();
            entidad.Property(movimiento => movimiento.Valor).HasPrecision(18, 2).IsRequired();
            entidad.Property(movimiento => movimiento.Saldo).HasPrecision(18, 2).IsRequired();
            entidad.HasIndex(movimiento => new { movimiento.CuentaId, movimiento.Fecha });
            entidad.HasOne<CuentaReporte>()
                .WithMany()
                .HasForeignKey(movimiento => movimiento.CuentaId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
