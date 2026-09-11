using Contratos.Compartidos.Eventos;
using Cuentas.Aplicacion.Contratos;
using Cuentas.Dominio.Entidades;
using Cuentas.Infraestructura.Persistencia;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Cuentas.Infraestructura.Mensajeria.Consumidores;

public sealed class ConsumidorClienteEliminado(IRepositorioClientesIntegracion repositorioClientes, ContextoCuentas contextoCuentas) : IConsumer<ClienteEliminado>
{
    public async Task Consume(ConsumeContext<ClienteEliminado> context)
    {
        ClienteEliminado eventoIntegracion = context.Message;
        bool yaRecibido = await contextoCuentas.EventosIntegracionRecibidos.AnyAsync(evento => evento.EventoId == eventoIntegracion.EventoId, context.CancellationToken);
        if (yaRecibido) { return; }
        ClienteIntegracion? cliente = await repositorioClientes.ObtenerPorIdAsync(eventoIntegracion.ClienteId, context.CancellationToken);

        if (cliente is not null) { repositorioClientes.Eliminar(cliente); }
        contextoCuentas.EventosIntegracionRecibidos.Add(new EventoIntegracionRecibido(eventoIntegracion.EventoId, nameof(ClienteEliminado)));
        await repositorioClientes.GuardarCambiosAsync(context.CancellationToken);
    }
}
