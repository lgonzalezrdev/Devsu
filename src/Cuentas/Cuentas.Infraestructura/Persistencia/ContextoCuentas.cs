using Cuentas.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cuentas.Infraestructura.Persistencia;

public sealed class ContextoCuentas(DbContextOptions<ContextoCuentas> opciones) : DbContext(opciones)
{
    public DbSet<Cuenta> Cuentas => Set<Cuenta>();

    public DbSet<Movimiento> Movimientos => Set<Movimiento>();

    public DbSet<ClienteIntegracion> ClientesIntegracion => Set<ClienteIntegracion>();
    public DbSet<EventoIntegracion> EventosIntegracion => Set<EventoIntegracion>();
    public DbSet<EventoIntegracionRecibido> EventosIntegracionRecibidos => Set<EventoIntegracionRecibido>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cuenta>(entidad =>
        {
            entidad.ToTable("Cuentas", "cuentas");
            entidad.HasKey(cuenta => cuenta.CuentaId);
            entidad.Property(cuenta => cuenta.CuentaId).ValueGeneratedNever();
            entidad.Property(cuenta => cuenta.ClienteId).IsRequired();
            entidad.Property(cuenta => cuenta.NumeroCuenta).HasMaxLength(20).IsRequired();
            entidad.HasIndex(cuenta => cuenta.NumeroCuenta).IsUnique();
            entidad.Property(cuenta => cuenta.TipoCuenta).HasConversion<string>().HasMaxLength(20).IsRequired();
            entidad.Property(cuenta => cuenta.SaldoInicial).HasPrecision(18, 2).IsRequired();
            entidad.Property(cuenta => cuenta.SaldoDisponible).HasPrecision(18, 2).IsRequired();
            entidad.Property(cuenta => cuenta.Estado).IsRequired();
            PropertyBuilder<byte[]> versionFila = entidad.Property(cuenta => cuenta.VersionFila).HasColumnName("VersionFila");
            if (Database.IsSqlServer())
            {
                versionFila.IsRowVersion();
            }
            else
            {
                versionFila.IsConcurrencyToken().ValueGeneratedNever();
            }
        });

        modelBuilder.Entity<Movimiento>(entidad =>
        {
            entidad.ToTable("Movimientos", "cuentas");
            entidad.HasKey(movimiento => movimiento.MovimientoId);
            entidad.Property(movimiento => movimiento.MovimientoId).ValueGeneratedNever();
            entidad.Property(movimiento => movimiento.Fecha).HasColumnType("datetime2(0)").IsRequired();
            entidad.Property(movimiento => movimiento.TipoMovimiento).HasConversion<string>().HasMaxLength(20).IsRequired();
            entidad.Property(movimiento => movimiento.Valor).HasPrecision(18, 2).IsRequired();
            entidad.Property(movimiento => movimiento.Saldo).HasPrecision(18, 2).IsRequired();
            entidad.HasOne<Cuenta>()
                .WithMany()
                .HasForeignKey(movimiento => movimiento.CuentaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ClienteIntegracion>(entidad =>
        {
            entidad.ToTable("ClientesIntegracion", "cuentas");
            entidad.HasKey(cliente => cliente.ClienteId);
            entidad.Property(cliente => cliente.ClienteId).ValueGeneratedNever();
            entidad.Property(cliente => cliente.Nombre).HasMaxLength(150).IsRequired();
            entidad.Property(cliente => cliente.Estado).IsRequired();
            entidad.Property(cliente => cliente.ActualizadoEn).HasColumnType("datetime2(0)").IsRequired();
        });

        modelBuilder.Entity<EventoIntegracion>(entidad => { entidad.ToTable("EventosIntegracion", "cuentas"); entidad.HasKey(evento => evento.EventoId); entidad.Property(evento => evento.Tipo).HasMaxLength(300).IsRequired(); entidad.Property(evento => evento.Contenido).IsRequired(); entidad.Property(evento => evento.OcurridoEn).HasColumnType("datetime2(0)"); entidad.Property(evento => evento.ProcesadoEn).HasColumnType("datetime2(0)"); entidad.Property(evento => evento.Error).HasMaxLength(2000); });
        modelBuilder.Entity<EventoIntegracionRecibido>(entidad => { entidad.ToTable("EventosIntegracionRecibidos", "cuentas"); entidad.HasKey(evento => evento.EventoId); entidad.Property(evento => evento.Tipo).HasMaxLength(300).IsRequired(); entidad.Property(evento => evento.RecibidoEn).HasColumnType("datetime2(0)"); });
    }
}
