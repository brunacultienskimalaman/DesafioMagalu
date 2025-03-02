using DesafioMagalu.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DesafioMagalu.Models
{
    public class Pedidos
    {
        [Key]
        [JsonProperty("order_id")]
        public int PedidoId { get; set; }

        [JsonProperty("total")]
        [JsonConverter(typeof(DecimalJsonConverter))]
        public decimal TotalPedido { get; set; }
        [JsonConverter(typeof(DateJsonConverter))]
        [JsonProperty("date")]
        public DateTime DataPedido  { get; set; }
        [JsonIgnore]
        public int UsuarioId { get; set; }
        [JsonIgnore]
        public Usuario Usuario { get; set; }
        [JsonProperty("products")]
        public List<Produto> Products { get; set; }
        [JsonIgnore]
        public ValidaImportacao Validacao { get; set; }
        public Pedidos()
        {}

        public Pedidos(int orderId, decimal total, DateTime date, List<Produto> products)
        {
            PedidoId = orderId;
            TotalPedido = total;
            DataPedido = date;
            Products = products;
        }

        public decimal GetTotal()
        {
            return Products.Sum(prod => prod.Valor);
        }
        public override string ToString()
        {
            string productsInfo = Products != null && Products.Any()
                ? string.Join(", ", Products.Select(p => p.ToString()))
                : "Nenhum produto";

            return $"OrderId: {PedidoId}, Total: {GetTotal():C}, Date: {DataPedido:dd/MM/yyyy}, Products: [{productsInfo}]";
        }


    }
}
