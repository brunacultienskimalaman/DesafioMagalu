namespace DesafioMagalu.Services
{
    public interface IZipService
    {
        Task<byte[]> CriarArquivoZipComProcessados(byte[] processados);
        Task<byte[]> CriarArquivoZipComTodos(byte[] processados, byte[] rejeitados, byte[] detalhamento);
    }
}
