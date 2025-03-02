using Azure;
using DesafioMagalu.Dtos;
using DesafioMagalu.Services;
using DesafioMagalu.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DesafioMagalu.Controllers
{
    [Route("api/magalu")]
    [ApiController]
    public class MagaluController : ControllerBase
    {
        private readonly IZipService _zipService;
        private readonly IMagaluService _magaluService;
        private readonly IArquivoValidator _arquivoValidator;
        private readonly IPedidoFiltroValidator _pedidoFiltroValidator;
        private readonly ILogger<MagaluController> _logger;

        public MagaluController(IZipService zipService, IMagaluService magaluService,
            IArquivoValidator arquivoValidator, IPedidoFiltroValidator pedidoFiltroValidator,
            ILogger<MagaluController> logger)
        {
            _zipService = zipService;
            _magaluService = magaluService;
            _arquivoValidator = arquivoValidator;
            _pedidoFiltroValidator = pedidoFiltroValidator;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadArquivo(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Arquivo inválido");

            string extensao = Path.GetExtension(file.FileName).ToLower();
            if (!_arquivoValidator.ValidarFormatoArquivo(extensao))
                return BadRequest("Formato de arquivo inválido. Apenas arquivos .txt são aceitos.");


            if (!_arquivoValidator.ValidarTamanhoArquivo(file.Length))
                return BadRequest("O arquivo excede o tamanho máximo permitido de 2MB.");


            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var response = _magaluService.ProcessarArquivo(memoryStream);

                byte[] processados = Encoding.UTF8.GetBytes(response.JsonProcessados);
                byte[] rejeitados = Encoding.UTF8.GetBytes(response.RegistrosRejeitados);
                byte[] msgRetorno = Encoding.UTF8.GetBytes(response.Mensagem);

                byte[] zipFile = await _zipService.CriarArquivoZipComTodos(processados, rejeitados, msgRetorno);

                return File(zipFile, "application/zip", "usuarios.zip");
            }
        }

        [HttpGet("pedidos")]
        public async Task<IActionResult> GetPedidos([FromQuery] PedidoFiltroDto filtro)
        {
            _logger.LogInformation("Iniciando execução do GetPedidos com filtro: DataInicio={DataInicio}, DataFim={DataFim}, PedidoId={PedidoId}",
                                   filtro.DataInicio, filtro.DataFim, filtro.PedidoId);

            filtro.DataInicio = filtro.DataInicio == DateTime.MinValue ? null : filtro.DataInicio;
            filtro.DataFim = filtro.DataFim == DateTime.MinValue ? null : filtro.DataFim;

            if (filtro.PedidoId.HasValue && !_pedidoFiltroValidator.ValidarPedidoId(filtro.PedidoId))
                return NotFound("O número do pedido deve ser maior que zero.");

            if (filtro.PedidoId.HasValue)
            {
                bool pedidoExiste = await _magaluService.PedidoExisteAsync(filtro.PedidoId.Value);
                if (!pedidoExiste)
                    return NotFound($"Pedido com ID {filtro.PedidoId} não encontrado.");
            }

            if (!_pedidoFiltroValidator.ValidarFiltrosDeData(filtro.DataInicio, filtro.DataFim))
            {
                _logger.LogWarning("Filtros de data inválidos: DataInicio={DataInicio}, DataFim={DataFim}", filtro.DataInicio, filtro.DataFim);
                return BadRequest("Quando um filtro de data é fornecido, ambos os campos DataInicio e DataFim devem ser preenchidos.");
            }

            if (!_pedidoFiltroValidator.ValidarIntervaloDatas(filtro.DataInicio, filtro.DataFim))
            {
                _logger.LogWarning("Intervalo de datas inválido: DataInicio={DataInicio}, DataFim={DataFim}", filtro.DataInicio, filtro.DataFim);
                return BadRequest("A data de início não pode ser posterior à data de fim.");
            }

            var pedidos = await _magaluService.ObterPedidosAsync(filtro);

            _logger.LogInformation("Pedidos obtidos com sucesso: {PedidosCount}", pedidos.Count());

            byte[] Consulta = Encoding.UTF8.GetBytes(pedidos);

            byte[] zipFile = await _zipService.CriarArquivoZipComProcessados(Consulta);

            return File(zipFile, "application/zip", "Consulta.zip");

        }

    }
}
