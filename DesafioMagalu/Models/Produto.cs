using DesafioMagalu.Utils;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace DesafioMagalu.Models
{
    public class Produto
    {
        [Key]
        [JsonProperty("product_id")]
        public int ProdutoId { get; set; }
        [JsonProperty("value")]
        [JsonConverter(typeof(DecimalJsonConverter))]
        public decimal Valor { get; set; }

        [JsonIgnore]
        public int PedidoId { get; set; }
        [JsonIgnore]
        public Pedidos Pedido { get; set; }
        [JsonIgnore]
        public ValidaImportacao validacao { get; set; }
        public Produto()
        {}
        public Produto(int productId, decimal value)
        {
            ProdutoId = productId;
            Valor = value;
        }
        public override string ToString()
        {
            return $"ProductId: {ProdutoId}, Value: {Valor:C}";
        }

    }
}
