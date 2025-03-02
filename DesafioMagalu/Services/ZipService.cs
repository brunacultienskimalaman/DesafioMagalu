using System.IO.Compression;

namespace DesafioMagalu.Services
{
    public class ZipService : IZipService
    {
       public async Task<byte[]> CriarArquivoZipComProcessados(byte[] processados)
        {
            return await CriarArquivoZip(processados, null, null);
        }
        public async Task<byte[]> CriarArquivoZipComTodos(byte[] processados, byte[] rejeitados, byte[] detalhamento)
        {
            return await CriarArquivoZip(processados, rejeitados, detalhamento);
        }

        private async Task<byte[]> CriarArquivoZip(byte[] processados, byte[]? rejeitados, byte[]? detalhamento)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    if (processados != null)
                    {
                        var processedEntry = zipArchive.CreateEntry("processados.json");
                        using (var entryStream = processedEntry.Open())
                        {
                            await entryStream.WriteAsync(processados, 0, processados.Length);
                        }
                    }

                    if (rejeitados != null)
                    {
                        var rejectedEntry = zipArchive.CreateEntry("rejeitados.json");
                        using (var entryStream = rejectedEntry.Open())
                        {
                            await entryStream.WriteAsync(rejeitados, 0, rejeitados.Length);
                        }
                    }

                    if (detalhamento != null)
                    {
                        var detalhamentoEntry = zipArchive.CreateEntry("detalhamento.json");
                        using (var entryStream = detalhamentoEntry.Open())
                        {
                            await entryStream.WriteAsync(detalhamento, 0, detalhamento.Length);
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }


    }
}
