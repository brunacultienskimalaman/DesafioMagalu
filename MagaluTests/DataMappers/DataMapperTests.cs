using Dapper;
using DesafioMagalu.Mapping;
using DesafioMagalu.Models;

namespace MagaluTests.DataMappers
{
    public class DataMapperTests
    {
        [Fact]
        public void MapToUsuario_IdUsuarioInvalido_DeveRetornarErro()
        {
            var content = "0000000B92                                 Ahmed Waters00000009850000000004     1056.2620211222";
            var mapper = new DataMapper();

            var usuario = mapper.MapToUsuario(content);

            Assert.False(usuario.validacao.Importado);
            Assert.Equal($"ID do usuario invalido: '{content.Substring(0, 10).Trim()}'. Deve ser um numero inteiro positivo.", usuario.validacao.Erro);
            
        }

        [Fact]
        public void MapToUsuario_NomeInvalido_DeveRetornarErro()
        {
            var content = "0000000092                                 Ahmed##Waters00000009850000000004     1056.262021122";
            var mapper = new DataMapper();

            var usuario = mapper.MapToUsuario(content);

            Assert.False(usuario.validacao.Importado);
            Assert.Equal("O nome contem caracteres invalidos. Apenas letras e espacos sao permitidos.", usuario.validacao.Erro);
        }

        [Fact]
        public void MapToPedidos_PedidoIdInvalido_DeveRetornarErro()
        {
            var content = "0000000021                               Alberto Murray0000000@250000000003      109.7520210901";  // PedidoId inválido
            var mapper = new DataMapper();

            var pedidos = mapper.MapToPedidos(content);

            Assert.False(pedidos.Validacao.Importado);
            Assert.Equal($"ID do pedido invalido: '{content.Substring(55, 10).Trim()}'. Deve ser um numero inteiro positivo.", pedidos.Validacao.Erro);
        }

        [Fact]
        public void MapToPedidos_DataPedidoInvalida_DeveRetornarErro()
        {
            var content = "0000000021                               Alberto Murray00000002250000000003      109.7520250901";  // Data inválida (no futuro)
            var mapper = new DataMapper();

            var pedidos = mapper.MapToPedidos(content);

            Assert.False(pedidos.Validacao.Importado);
            Assert.Equal($"A data do pedido nao pode ser no futuro. Data fornecida: '{content.Substring(87, 8).Trim()}'.", pedidos.Validacao.Erro);
        }
        [Fact]
        public void MapToProduto_ProdutoIdInvalido_DeveRetornarErro()
        {
            var content = "0000000021                               Alberto Murray0000000225000000000#      109.7520210901";  // ProdutoId inválido
            var mapper = new DataMapper();

            var produto = mapper.MapToProduto(content);

            Assert.False(produto.validacao.Importado);
            Assert.Equal($"ID do produto invalido: '{content.Substring(65, 10).Trim()}'. Deve ser um numero inteiro positivo.", produto.validacao.Erro);
        }
        [Fact]
        public void MapToProduto_ValorInvalido_DeveRetornarErro()
        {
            var content = "0000000021                               Alberto Murray00000002250000000003      -09.7520210901";  // Valor inválido
            var mapper = new DataMapper();

            var produto = mapper.MapToProduto(content);

            Assert.False(produto.validacao.Importado);
            Assert.Equal($"Valor invalido: '{content.Substring(77, 10).Trim()}'. Deve ser um numero decimal positivo ou zero.", produto.validacao.Erro);
        }


    }
}
