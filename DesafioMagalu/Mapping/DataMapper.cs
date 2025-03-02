using DesafioMagalu.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DesafioMagalu.Mapping
{
    public class DataMapper : IDataMapper
    {
        #region UserId
        public Usuario MapToUsuario(string content)
        {
            var validacaoUsuario = new ValidaImportacao(true);
            var usr = new Usuario
            {
                UsuarioId = ValidarUsuario(content.Substring(0, 10).Trim(), validacaoUsuario),
                Nome = ValidarNome(content.Substring(10, 45).Trim(),validacaoUsuario),
                validacao = validacaoUsuario,
            };
            return usr;
           
        }
        private int ValidarUsuario(string content, ValidaImportacao validacao)
        {
            if (int.TryParse(content, out int UsuarioId) && UsuarioId > 0)
            {
                return UsuarioId;
            }
            else
            {
                validacao.Erro = ($"ID do usuario invalido: '{content}'. Deve ser um numero inteiro positivo.");
                validacao.ConteudoLinha = content;
                validacao.Importado = false;
                return 0;
            }
        }
        private string ValidarNome(string content, ValidaImportacao validacao)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                validacao.Erro = ("O nome nao pode ser vazio ou conter apenas espacos.");
                validacao.ConteudoLinha = content;
                validacao.Importado = false;
                return string.Empty; 
            }

            var regex = new Regex(@"^[a-zA-ZÀ-ÿ\s]+$");
            if (!regex.IsMatch(content))
            {
                validacao.Erro = ("O nome contem caracteres invalidos. Apenas letras e espacos sao permitidos.");
                validacao.ConteudoLinha = content;
                validacao.Importado = false;
                return string.Empty;
            }
            return content;
        }
        #endregion
        #region Pedidos
        public Pedidos MapToPedidos(string content)
        {
            var validacaoUsuario = new ValidaImportacao(true);
            var pedidos = new Pedidos
                {
                    PedidoId = ValidarPedidoId(content.Substring(55, 10).Trim(), validacaoUsuario),
                    DataPedido = ValidarDataPedido(content.Substring(87, 8).Trim(), validacaoUsuario),
                    Validacao = validacaoUsuario
            };
                return pedidos;
        }
        private int ValidarPedidoId(string content, ValidaImportacao validacao)
        {
            if (int.TryParse(content, out int pedidoId) && pedidoId >= 0)
            {
                return pedidoId;
            }
            else
            {
                validacao.Erro = ($"ID do pedido invalido: '{content}'. Deve ser um numero inteiro positivo.");
                validacao.ConteudoLinha = content;
                validacao.Importado = false;
                return 0; 
            }
        }
        private DateTime ValidarDataPedido(string content, ValidaImportacao validacao)
        {
            if (DateTime.TryParseExact(content, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataPedido))
            {
                if (dataPedido > DateTime.Now)
                {
                    validacao.Erro = ($"A data do pedido nao pode ser no futuro. Data fornecida: '{content}'.");
                    validacao.ConteudoLinha = content;
                    validacao.Importado = false;
                    return DateTime.MinValue; 
                }

                return dataPedido;
            }
            else
            {
                validacao.Erro = ($"Data do pedido invalida: '{content}'. O formato correto e yyyyMMdd.");
                validacao.ConteudoLinha = content;
                validacao.Importado = false;
                return DateTime.MinValue; 
            }
        }
        #endregion
        #region Produto
        public Produto MapToProduto(string content)
        {
            var validacaoUsuario = new ValidaImportacao(true);
            var produto = new Produto
            {
                ProdutoId = ValidarProdutoId(content.Substring(65, 10).Trim(),validacaoUsuario),
                Valor = ValidarValor(content.Substring(77, 10).Trim(),validacaoUsuario),
                validacao = validacaoUsuario
            };

            return produto;
        }
        private int ValidarProdutoId(string content, ValidaImportacao validacao)
        {
            if (int.TryParse(content, out int produtoId) && produtoId >= 0)
            {
                return produtoId;
            }
            else
            {
                validacao.Erro = ($"ID do produto invalido: '{content}'. Deve ser um numero inteiro positivo.");
                validacao.ConteudoLinha = content;
                validacao.Importado = false;
                return 0; 
            }
        }
        private decimal ValidarValor(string content, ValidaImportacao validacao)
        {
            if (decimal.TryParse(content, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valor) && valor >= 0)
            {
                return valor;
            }
            else
            {
                validacao.Erro = ($"Valor invalido: '{content}'. Deve ser um numero decimal positivo ou zero.");
                validacao.ConteudoLinha = content;
                validacao.Importado = false;
                return 0m; 
            }
        }
        #endregion
    }
}
