using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace DesafioMagalu.Models
{
    public class Usuario
    {
        [Key]
        
        [JsonProperty("user_id")]
        public int UsuarioId { get; set; }
        [JsonProperty("name")]
        public string Nome { get;  set; }
        [JsonProperty("orders")]
        public List<Pedidos> Orders { get; set; }
        [JsonIgnore]
        public ValidaImportacao validacao { get; set; }
        public Usuario()
        {}
        
        public override string ToString()
        {
            string ordersInfo = Orders != null && Orders.Any()
                ? string.Join(", ", Orders.Select(o => o.ToString()))
                : "Nenhum pedido";

            return $"UserID: {UsuarioId}, Name: {Nome}, Orders: [{ordersInfo}]";
        }
    }
}
