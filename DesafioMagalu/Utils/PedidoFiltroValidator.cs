namespace DesafioMagalu.Utils
{
    public class PedidoFiltroValidator : IPedidoFiltroValidator
    {
        public bool ValidarPedidoId(int? pedidoId)
        {
            return pedidoId.HasValue && pedidoId > 0;
        }

        public bool ValidarIntervaloDatas(DateTime? dataInicio, DateTime? dataFim)
        {
            if (dataInicio.HasValue && dataFim.HasValue)
            {
                return dataInicio <= dataFim;
            }
            return true;
        }

        public bool ValidarFiltrosDeData(DateTime? dataInicio, DateTime? dataFim)
        {
            if (dataInicio.HasValue && !dataFim.HasValue)
                return false; 

            if (!dataInicio.HasValue && dataFim.HasValue)
                return false; 

            return true;
        }

    }
}
