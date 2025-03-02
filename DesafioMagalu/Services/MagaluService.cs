using DesafioMagalu.Dtos;
using DesafioMagalu.Repositories;

namespace DesafioMagalu.Services
{
    public class MagaluService : IMagaluService
    {
        private readonly IMagaluRepository _magaluRepository;

        public MagaluService(IMagaluRepository magaluRepository)
        {
            _magaluRepository = magaluRepository;
        }

        public async Task<string> ObterPedidosAsync(PedidoFiltroDto filtro)
        {
            return await _magaluRepository.ObterPedidosAsync(filtro);
        }

        public UploadResponseDto ProcessarArquivo(Stream fileStream)
        {
            return _magaluRepository.ProcessarArquivo(fileStream);
        }

        public async Task<bool> PedidoExisteAsync(int? idPedido)
        {
            return await _magaluRepository.PedidoExisteAsync(idPedido);
        }
    }
}
