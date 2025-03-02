namespace DesafioMagalu.Utils
{
    public interface IPedidoFiltroValidator
    {
        bool ValidarPedidoId(int? pedidoId);
        bool ValidarIntervaloDatas(DateTime? dataInicio, DateTime? dataFim);
        bool ValidarFiltrosDeData(DateTime? dataInicio, DateTime? dataFim);
    }
}
