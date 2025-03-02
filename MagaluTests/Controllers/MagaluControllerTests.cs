using DesafioMagalu.Controllers;
using DesafioMagalu.Dtos;
using DesafioMagalu.Models;
using DesafioMagalu.Services;
using DesafioMagalu.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace MagaluTests.Controllers
{
    public class MagaluControllerTests
    {
        public Mock<IZipService> ZipServiceMock { get; private set; }
        public Mock<IMagaluService> MagaluServiceMock { get; private set; }
        public Mock<IArquivoValidator> ArquivoValidatorMock { get; private set; }
        public Mock<IPedidoFiltroValidator> PedidoFiltroValidatorMock { get; private set; }
        public Mock<ILogger<MagaluController>> LoggerMock { get; private set; }  // Mock para o Logger
        public MagaluController Controller { get; private set; }

        public MagaluControllerTests()
        {
            ZipServiceMock = new Mock<IZipService>();
            MagaluServiceMock = new Mock<IMagaluService>();
            ArquivoValidatorMock = new Mock<IArquivoValidator>();
            PedidoFiltroValidatorMock = new Mock<IPedidoFiltroValidator>();
            LoggerMock = new Mock<ILogger<MagaluController>>(); 

            Controller = new MagaluController(
                ZipServiceMock.Object,
                MagaluServiceMock.Object,
                ArquivoValidatorMock.Object,
                PedidoFiltroValidatorMock.Object,
                LoggerMock.Object
            );
        }

        [Fact]
        public async Task UploadArquivo_ArquivoInvalido_DeveRetornarBadRequest()
        {
            IFormFile file = null;
            var result = await Controller.UploadArquivo(file);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Arquivo inválido", badRequestResult.Value);
        }

        [Fact]
        public async Task UploadArquivo_FormatoInvalido_DeveRetornarBadRequest()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("arquivo.csv");
            fileMock.Setup(f => f.Length).Returns(1024);

            ArquivoValidatorMock.Setup(v => v.ValidarFormatoArquivo(".csv")).Returns(false);

            var result = await Controller.UploadArquivo(fileMock.Object);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Formato de arquivo inválido. Apenas arquivos .txt são aceitos.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetPedidos_PedidoIdInvalido_DeveRetornarNotFound()
        {
            var filtro = new PedidoFiltroDto { PedidoId = 0 }; // PedidoId inválido

            PedidoFiltroValidatorMock
                .Setup(v => v.ValidarPedidoId(It.IsAny<int>()))
                .Returns(false); // Simulando que o PedidoId é inválido

            var result = await Controller.GetPedidos(filtro);
            
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("O número do pedido deve ser maior que zero.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetPedidos_PedidoIdNaoExistente_DeveRetornarNotFound()
        {
            var filtro = new PedidoFiltroDto { PedidoId = 123 }; // PedidoId que não existe

            PedidoFiltroValidatorMock
                .Setup(v => v.ValidarPedidoId(It.IsAny<int>()))
                .Returns(true); // PedidoId válido

            MagaluServiceMock
                .Setup(s => s.PedidoExisteAsync(It.IsAny<int>()))
                .ReturnsAsync(false); // Simulando que o pedido não existe

            var result = await Controller.GetPedidos(filtro);
            
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Pedido com ID {filtro.PedidoId} não encontrado.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetPedidos_IntervaloDatasInvalido_DeveRetornarBadRequest()
        {

            var filtro = new PedidoFiltroDto { DataInicio = DateTime.Now.AddDays(1), DataFim = DateTime.Now }; 

            PedidoFiltroValidatorMock
                .Setup(v => v.ValidarIntervaloDatas(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Callback<DateTime?, DateTime?>((inicio, fim) =>
                {
                    Console.WriteLine($"[TESTE] ValidarIntervaloDatas chamado com: DataInicio={inicio}, DataFim={fim}");
                    LoggerMock.Object.LogInformation($"[TESTE] ValidarIntervaloDatas chamado com: DataInicio={inicio}, DataFim={fim}"); 
                })
                .Returns(false);

            LoggerMock.Object.LogInformation("Iniciando o teste GetPedidos_IntervaloDatasInvalido_DeveRetornarBadRequest com filtro: DataInicio={0}, DataFim={1}", filtro.DataInicio, filtro.DataFim);

            var result = await Controller.GetPedidos(filtro);
            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("A data de início não pode ser posterior à data de fim.", badRequestResult.Value);

            LoggerMock.Object.LogInformation("Resultado do teste: {StatusCode} com mensagem: {Mensagem}", badRequestResult.StatusCode, badRequestResult.Value);
        }


        [Fact]
        public async Task GetPedidos_FiltrosDeDataNaoPreenchidos_DeveRetornarBadRequest()
        {
            var filtro = new PedidoFiltroDto { DataInicio = DateTime.Now, DataFim = null }; // DataFim não preenchida

            PedidoFiltroValidatorMock
                .Setup(v => v.ValidarFiltrosDeData(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(false); 

            var result = await Controller.GetPedidos(filtro);
            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Quando um filtro de data é fornecido, ambos os campos DataInicio e DataFim devem ser preenchidos.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetPedidos_Valido_DeveRetornarOkComJson()
        {
            var filtro = new PedidoFiltroDto
            {
                DataInicio = DateTime.Parse("2021-12-08")

            };

            var pedidosMock = new List<Usuario>
    {
        new Usuario
        {
            UsuarioId = 21,
            Nome = "Alberto Murray",
            Orders = new List<Pedidos>
            {
                new Pedidos
                {
                    PedidoId = 217,
                    TotalPedido = 140734.00m,
                    DataPedido = DateTime.Parse("2021-12-08"),
                    Products = new List<Produto>
                    {
                        new Produto { ProdutoId = 2, Valor = 140734.00m }
                    }
                }
            }
        }
    };

            var pedidosJsonMock = JsonConvert.SerializeObject(pedidosMock); 

            MagaluServiceMock
                .Setup(s => s.ObterPedidosAsync(It.IsAny<PedidoFiltroDto>()))
                .ReturnsAsync(pedidosJsonMock); 

            var result = await Controller.GetPedidos(filtro);

            var okResult = Assert.IsType<OkObjectResult>(result); 

            var returnJson = Assert.IsType<string>(okResult.Value); 

            var returnUsuarios = JsonConvert.DeserializeObject<List<Usuario>>(returnJson); 
            Assert.Single(returnUsuarios); 

            var usuario = returnUsuarios[0];
            Assert.Equal(21, usuario.UsuarioId); 
            Assert.Equal("Alberto Murray", usuario.Nome); 

            var pedido = usuario.Orders[0];
            Assert.Equal(217, pedido.PedidoId); 
            Assert.Equal(140734.00m, pedido.GetTotal()); 
            Assert.Equal(DateTime.Parse("2021-12-08"), pedido.DataPedido); 

            Assert.Equal(1, pedido.Products.Count); 
            Assert.Equal(2, pedido.Products[0].ProdutoId); 
            Assert.Equal(140734.00m, pedido.Products[0].Valor); 
        }
    }
}
