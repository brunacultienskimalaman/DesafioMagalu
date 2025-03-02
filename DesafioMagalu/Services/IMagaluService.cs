using DesafioMagalu.Dtos;
using DesafioMagalu.Models;

namespace DesafioMagalu.Services
{
    public interface IMagaluService
    {
        Task<string> ObterPedidosAsync(PedidoFiltroDto filtro);
        UploadResponseDto ProcessarArquivo(Stream fileStream);
        Task<bool> PedidoExisteAsync(int? idPedido);
    }
}
