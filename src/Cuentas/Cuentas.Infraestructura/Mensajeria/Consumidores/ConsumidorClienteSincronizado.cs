using Contratos.Compartidos.Eventos;
using Cuentas.Aplicacion.Contratos;
using Cuentas.Dominio.Entidades;
using MassTransit;

namespace Cuentas.Infraestructura.Mensajeria.Consumidores;

public sealed class ConsumidorClienteSincronizado(IRepositorioClientesIntegracion repositorioClientes) : IConsumer<ClienteSincronizado>
{
    public async Task Consume(ConsumeContext<ClienteSincronizado> context)
    {
        ClienteSincronizado eventoIntegracion = context.Message;
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

        await repositorioClientes.GuardarCambiosAsync(context.CancellationToken);
    }
}
