namespace DesafioMagalu.Dtos
{
    public class PedidoFiltroDto
    {
        //lembrar de por na dpocumentacao q deixei tudo opcional para o usuario pesquisar confoirme a necessidade
        public int? PedidoId { get; set; } 
        public DateTime? DataInicio { get; set; } 
        public DateTime? DataFim { get; set; } 
    }
}
