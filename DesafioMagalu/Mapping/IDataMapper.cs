using DesafioMagalu.Models;

namespace DesafioMagalu.Mapping
{
    public interface IDataMapper
    {
        Usuario MapToUsuario(string content);
        Pedidos MapToPedidos(string content);
        Produto MapToProduto(string content);
    }
}
