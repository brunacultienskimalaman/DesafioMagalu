using DesafioMagalu.Dtos;

namespace DesafioMagalu.Repositories
{
    public interface IMagaluRepository
    {
        Task<string> ObterPedidosAsync(PedidoFiltroDto filtro);
        UploadResponseDto ProcessarArquivo(Stream fileStream);
        Task<bool> PedidoExisteAsync(int? idPedido);
    }
}
