namespace DesafioMagalu.Models
{
    public class ValidaImportacao
    {
        public string Erro { get; set; }
        public string ConteudoLinha { get; set; }
        public bool Importado { get; set; }
        public ValidaImportacao(bool importado)
        {
            this.Importado = importado;
        }
    }
}
