namespace DesafioMagalu.Utils
{
    public interface IArquivoValidator
    {
        bool ValidarFormatoArquivo(string extensao);
        bool ValidarTamanhoArquivo(long tamanhoArquivo);
    }
}
