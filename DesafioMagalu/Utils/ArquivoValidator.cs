namespace DesafioMagalu.Utils
{
    public class ArquivoValidator : IArquivoValidator
    {
        public bool ValidarFormatoArquivo(string extensao)
        {
            return extensao == ".txt";
        }

        public bool ValidarTamanhoArquivo(long tamanhoArquivo)
        {
            long maxFileSize = 2 * 1024 * 1024; // 2MB
            return tamanhoArquivo <= maxFileSize;
        }
    }
}
