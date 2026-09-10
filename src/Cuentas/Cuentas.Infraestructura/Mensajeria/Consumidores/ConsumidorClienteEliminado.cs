using Contratos.Compartidos.Eventos;
using Cuentas.Aplicacion.Contratos;
using Cuentas.Dominio.Entidades;
using MassTransit;

namespace Cuentas.Infraestructura.Mensajeria.Consumidores;

public sealed class ConsumidorClienteEliminado(IRepositorioClientesIntegracion repositorioClientes) : IConsumer<ClienteEliminado>
{
    public async Task Consume(ConsumeContext<ClienteEliminado> context)
    {
        ClienteEliminado eventoIntegracion = context.Message;
        ClienteIntegracion? cliente = await repositorioClientes.ObtenerPorIdAsync(eventoIntegracion.ClienteId, context.CancellationToken);

        if (cliente is not null)
        {
            repositorioClientes.Eliminar(cliente);
            await repositorioClientes.GuardarCambiosAsync(context.CancellationToken);
        }
    }
}
