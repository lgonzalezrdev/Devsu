using Cuentas.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Cuentas.Infraestructura.Persistencia;

public sealed class ContextoCuentas(DbContextOptions<ContextoCuentas> opciones) : DbContext(opciones)
{
    public DbSet<Cuenta> Cuentas => Set<Cuenta>();

    public DbSet<Movimiento> Movimientos => Set<Movimiento>();

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
    }
}
