using DesafioMagalu.Data;
using DesafioMagalu.Dtos;
using DesafioMagalu.Mapping;
using DesafioMagalu.Models;
using Newtonsoft.Json;
using System.Text.Json;

namespace DesafioMagalu.Repositories
{
    public class MagaluRepository : IMagaluRepository
    {

        private readonly IDataMapper _dataMapper;
        private readonly DatabaseService _dbService;

        public MagaluRepository(IDataMapper dataMapper, DatabaseService dbService)
        {
            _dataMapper = dataMapper;
            _dbService = dbService;
        }

        public async Task<string> ObterPedidosAsync(PedidoFiltroDto filtro)
        {
                var result = await _dbService.ObterDadosDoBancoAsync(filtro);
                List<Usuario> retorno = new List<Usuario>();

                foreach (var row in result)
                {
                    Usuario usr = new Usuario
                    {
                        UsuarioId = row.USER_ID,
                        Nome = row.NAME,
                        Orders = new List<Pedidos>()
                    };

                    Pedidos pedido = new Pedidos
                    {
                        PedidoId = row.ORDER_ID,
                        DataPedido = row.DATE,
                        Products = new List<Produto>()
                    };

                    string[] pedidosArray = row.PRODUCTS.Contains(",") ? row.PRODUCTS.Split(",") : new string[] { row.PRODUCTS };

                    foreach (var pedidoStr in pedidosArray)
                    {
                        var prod = pedidoStr.Split(":");
                        Produto produto = new Produto
                        {
                            PedidoId = row.ORDER_ID,
                            ProdutoId = int.Parse(prod[0]),
                            Valor = decimal.Parse(prod[1])
                        };

                        pedido.Products.Add(produto);
                        pedido.TotalPedido = pedido.GetTotal();
                    }

                    usr.Orders.Add(pedido);
                    retorno.Add(usr);
                }
                return JsonConvert.SerializeObject(retorno, Formatting.Indented);
            }
    
        public UploadResponseDto ProcessarArquivo(Stream fileStream)
        {
            List<Usuario> registrosProcessados = new List<Usuario>();
            List<string> registrosRejeitadas = new List<string>();
           
            Usuario usr = new Usuario();
            Pedidos pedidos = new Pedidos();
            Produto produto = new Produto();

            var msgRetorno = "";

            using (var reader = new StreamReader(fileStream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string content = line;
                    if (!string.IsNullOrEmpty(content) && content.Length == 95)
                    {
                        try
                        {
                            usr = _dataMapper.MapToUsuario(content);
                            if (usr.validacao.Importado)
                            {
                                pedidos = _dataMapper.MapToPedidos(content);
                                if (pedidos.Validacao.Importado)
                                {
                                    produto = _dataMapper.MapToProduto(content);
                                    if (produto.validacao.Importado)
                                    {
                                        pedidos.Products = new List<Produto> { produto };
                                        pedidos.TotalPedido = pedidos.GetTotal();

                                        usr.Orders = new List<Pedidos> { pedidos };
                                        registrosProcessados.Add(usr);
                                    }
                                    else
                                    {
                                        registrosRejeitadas.Add(produto.validacao.ConteudoLinha);
                                        registrosRejeitadas.Add(produto.validacao.Erro);
                                    }
                                }
                                else
                                {
                                    registrosRejeitadas.Add(pedidos.Validacao.ConteudoLinha);
                                    registrosRejeitadas.Add(pedidos.Validacao.Erro);
                                }
                            }
                            else
                            {
                                registrosRejeitadas.Add(usr.validacao.ConteudoLinha);
                                registrosRejeitadas.Add(usr.validacao.Erro);
                            }
                        }
                        catch (Exception e)
                        {
                            registrosRejeitadas.Add(content);
                            registrosRejeitadas.Add(e.Message);
                        }
                    }
                    else
                    {
                        registrosRejeitadas.Add(content);
                        registrosRejeitadas.Add("Linha rejeitada por ter menos caracteres");
                    }
                }
                msgRetorno = $"Total de registros processados: {registrosProcessados.Count}, total de registros rejeitados {registrosRejeitadas.Count / 2}";
                return AgruparUsuariosPorPedidosEProdutos(registrosProcessados, registrosRejeitadas, msgRetorno);
            }
        }
        public UploadResponseDto AgruparUsuariosPorPedidosEProdutos(List<Usuario> registrosProcessados, List<string> registrosRejeitadas, string msgRetorno)
        {
            var retorno = new UploadResponseDto
            {
                JsonProcessados = SerializarParaJson(registrosProcessados),
                RegistrosRejeitados = SerializarParaJson(registrosRejeitadas),
                Mensagem = msgRetorno
            };
            _dbService.CriarUsuarioPedidoEProdutos(registrosProcessados);

            return retorno;
        }
        private string SerializarParaJson(object objeto)
        {
            return System.Text.Json.JsonSerializer.Serialize(objeto, new JsonSerializerOptions { WriteIndented = true });
        }
        public async Task<bool> PedidoExisteAsync(int? idPedido)
        {
            return await _dbService.PedidoExisteAsync(idPedido);
        }
    }
}
