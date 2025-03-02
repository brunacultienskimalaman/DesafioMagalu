using DesafioMagalu.Models;

namespace DesafioMagalu.Dtos
{
    public class UploadResponseDto
    {
        public string Mensagem { get; set; }
        public string JsonProcessados { get; set; }
        public string RegistrosRejeitados { get; set; }
    }
}
