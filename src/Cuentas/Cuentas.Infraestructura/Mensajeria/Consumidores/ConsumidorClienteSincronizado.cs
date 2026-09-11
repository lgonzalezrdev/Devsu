using Contratos.Compartidos.Eventos;
using Cuentas.Aplicacion.Contratos;
using Cuentas.Dominio.Entidades;
using Cuentas.Infraestructura.Persistencia;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cuentas.Infraestructura.Mensajeria.Consumidores;

public sealed class ConsumidorClienteSincronizado(IRepositorioClientesIntegracion repositorioClientes, ContextoCuentas contextoCuentas) : IConsumer<ClienteSincronizado>
{
    public async Task Consume(ConsumeContext<ClienteSincronizado> context)
    {
        ClienteSincronizado eventoIntegracion = context.Message;
        bool yaRecibido = await contextoCuentas.EventosIntegracionRecibidos.AnyAsync(evento => evento.EventoId == eventoIntegracion.EventoId, context.CancellationToken);
        if (yaRecibido) { return; }
        ClienteIntegracion? cliente = await repositorioClientes.ObtenerPorIdAsync(eventoIntegracion.ClienteId, context.CancellationToken);

        if (cliente is null)
        {
            ClienteIntegracion nuevoCliente = new(
                eventoIntegracion.ClienteId,
                eventoIntegracion.Nombre,
                eventoIntegracion.Estado,
                eventoIntegracion.FechaOcurrencia);
            await repositorioClientes.AgregarAsync(nuevoCliente, context.CancellationToken);
        }
        else
        {
            cliente.Actualizar(eventoIntegracion.Nombre, eventoIntegracion.Estado, eventoIntegracion.FechaOcurrencia);
        }

        contextoCuentas.EventosIntegracionRecibidos.Add(new EventoIntegracionRecibido(eventoIntegracion.EventoId, nameof(ClienteSincronizado)));
        await repositorioClientes.GuardarCambiosAsync(context.CancellationToken);
    }
}
