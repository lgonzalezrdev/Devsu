namespace Cuentas.Aplicacion.Modelos;

public sealed record ConsultaCuentas(int Pagina = 1, int TamanoPagina = 20, Guid? ClienteId = null, bool? Estado = null);
public sealed record ConsultaMovimientos(int Pagina = 1, int TamanoPagina = 20, DateTime? FechaInicio = null, DateTime? FechaFin = null);
public sealed record ResultadoPaginado<T>(IReadOnlyCollection<T> Elementos, int Pagina, int TamanoPagina, int TotalRegistros)
{
    public int TotalPaginas => (int)Math.Ceiling((double)TotalRegistros / TamanoPagina);
}
