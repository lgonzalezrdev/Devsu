using Clientes.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infraestructura.Persistencia;

public sealed class ContextoClientes(DbContextOptions<ContextoClientes> opciones) : DbContext(opciones)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();

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
    }
}
