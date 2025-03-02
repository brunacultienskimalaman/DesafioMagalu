using Dapper;
using DesafioMagalu.Dtos;
using DesafioMagalu.Models;
using Microsoft.Data.SqlClient;
using System.Text;

namespace DesafioMagalu.Data
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<bool> CriarUsuarioPedidoEProdutos(List<Usuario> registrosProcessados)
        {
            using var db = new SqlConnection(_connectionString);
            await db.OpenAsync();
            using var transaction = db.BeginTransaction();

            try
            {
                foreach (var usuario in registrosProcessados)
                {
                    var usuarioExistente = await db.QueryFirstOrDefaultAsync<string>("SELECT USERID FROM USUARIOS WHERE USERID = @USERID OR NAME = @NAME",
                     new { usuario.UsuarioId, usuario.Nome }, transaction);

                    if (usuarioExistente == null)
                    {
                        var sqlUsuario = "INSERT INTO USUARIOS (USERID, NAME) OUTPUT INSERTED.USERID  VALUES (@USERID, @NAME)";
                        usuario.UsuarioId = await db.ExecuteScalarAsync<int>(sqlUsuario, usuario, transaction); // Pega o UserID gerado
                    }

                    foreach (var pedido in usuario.Orders)
                    {

                        var pedidoExistente = await db.QueryFirstOrDefaultAsync<int>(
                        "SELECT ORDERID FROM PEDIDOS WHERE ORDERID = @ORDERID",
                       new { pedido.PedidoId }, transaction);

                        if (pedidoExistente == 0)
                        {
                            pedido.UsuarioId = usuario.UsuarioId;
                            var sqlPedido = @"
                            INSERT INTO PEDIDOS (ORDERID,TOTAL, DATE, USUARIOID)  OUTPUT INSERTED.ORDERID 
                            VALUES (@ORDERID,@TOTAL, @DATE, @USUARIOID)";
                            pedido.PedidoId = await db.ExecuteScalarAsync<int>(sqlPedido, pedido, transaction); // Pega o OrderId gerado
                        }

                        foreach (var produto in pedido.Products)
                        {

                            string checkQuery = "SELECT COUNT(1) FROM PRODUTOS WHERE PRODUCTID = @PRODUCTID AND PEDIDOID = @PEDIDOID";
                            int existe = await db.QueryFirstOrDefaultAsync<int>(checkQuery, new { ProductId = produto.ProdutoId, PedidoId = pedido.PedidoId },
                       transaction);

                            if (existe == 0)
                            {
                                var sqlProduto = "INSERT INTO PRODUTOS (PRODUCTID,VALUE, PEDIDOID) VALUES (@PRODUCTID,@VALUE, @PEDIDOID)";
                                produto.PedidoId = pedido.PedidoId; 
                                await db.ExecuteAsync(sqlProduto, produto, transaction);
                            }
                        }
                    }
                }
                transaction.Commit();
                return true;

            }
            catch (Exception e)
            {
                transaction.Rollback();
                return true;
            }
        }
        public async Task<IEnumerable<dynamic>> ObterDadosDoBancoAsync(PedidoFiltroDto filtro)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = new StringBuilder(@"
                    SELECT 
                        USR.USERID AS USER_ID,
                        USR.NAME AS NAME,
                        PED.ORDERID AS ORDER_ID,
                        PED.DATE AS DATE,
                        STRING_AGG(CONCAT(PROD.PRODUCTID, ':', PROD.VALUE), ',') AS PRODUCTS
                    FROM USUARIOS USR
                    INNER JOIN PEDIDOS PED ON PED.USUARIOID = USR.USERID
                    INNER JOIN PRODUTOS PROD ON PROD.PEDIDOID = PED.ORDERID
                    WHERE 1=1 ");

                var parameters = new DynamicParameters();

                if (filtro.PedidoId.HasValue)
                {
                    sql.Append(" AND PED.ORDERID = @IdPedido");
                    parameters.Add("@IdPedido", filtro.PedidoId.Value);
                }

                if (filtro.DataInicio.HasValue && filtro.DataFim.HasValue)
                {
                    sql.Append(" AND PED.DATE BETWEEN @DataInicio AND @DataFim");
                    parameters.Add("@DataInicio", filtro.DataInicio.Value);
                    parameters.Add("@DataFim", filtro.DataFim.Value);
                }

                sql.Append(@"
                    GROUP BY USR.USERID, USR.NAME, PED.ORDERID, PED.DATE
                    ORDER BY USR.USERID, PED.ORDERID;");

                return await connection.QueryAsync(sql.ToString(), parameters);
            }
        }
        public async Task<bool> PedidoExisteAsync(int? idPedido)
        {
            if (idPedido != null)
            {
                var query = "SELECT COUNT(1) FROM PEDIDOS WHERE ORDERID = @IDPEDIDO";
                using var db = new SqlConnection(_connectionString);
                var result = await db.ExecuteScalarAsync<int>(query, new { IdPedido = idPedido });

                return result > 0;
            }
            return false;
        }
    }
}
