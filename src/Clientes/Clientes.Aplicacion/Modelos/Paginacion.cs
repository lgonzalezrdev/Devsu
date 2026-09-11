namespace Clientes.Aplicacion.Modelos;

public sealed record ConsultaClientes(int Pagina = 1, int TamanoPagina = 20, string? Nombre = null, bool? Estado = null);
public sealed record ResultadoPaginado<T>(IReadOnlyCollection<T> Elementos, int Pagina, int TamanoPagina, int TotalRegistros)
{
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / TamanoPagina);
}
