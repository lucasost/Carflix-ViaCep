using Carflix.Controllers;
using Carflix.Services;
using Carflix.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Carflix.Test
{
    public class ConsultarControllerTest
    {
        private readonly CarflixContext _db;

        private readonly Mock<IViaCepApiService> _viaCepService;

        private readonly CepController _controller;

        private readonly TempDataDictionary _tempdata;



        public ConsultarControllerTest()
        {
            var optionsBuilder = new DbContextOptionsBuilder<CarflixContext>();
            optionsBuilder.UseInMemoryDatabase("CarflixContext");
            var options = optionsBuilder.Options;

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            _db = new CarflixContext(options);
            _viaCepService = new Mock<IViaCepApiService>();
            _controller = new CepController(_db, _viaCepService.Object);

            _controller.TempData = tempData;
        }

        [Fact]
        public async Task Consultar_Post_QuandoHaErroNoRetornoDoCepMensagemDeveSerExibidaAoUsuario()
        {
            // Arrange
            _viaCepService.Setup(x => x.BuscaLogradouro(It.IsAny<string>())).Returns(Task.FromResult(new ViewModels.ViaCepResponse() { Erro = true }));

            var viewModel = new ConsultaCepViewModel()
            {
                Cep = "00000-000"
            };

            // Act
            var result = await _controller.Consultar(viewModel, "Consultar");

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var modelResult = Assert.IsType<ConsultaCepViewModel>(viewResult.Model);
            Assert.Equal(viewModel.Cep, modelResult.Cep);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Collection(_controller.ModelState.Values.SelectMany(a => a.Errors), item => Assert.Equal("CEP inválido ou não encontrado", item.ErrorMessage));
        }

        [Fact]
        public async Task Consultar_Post_QuandoCepJaEstaCadastradoDeveSerExibidaAoUsuario()
        {
            // Arrange
            _db.Logradouros.Add(new Models.Logradouro()
            {
                Cep = "12345-678"
            });
            await _db.SaveChangesAsync();

            _viaCepService.Setup(x => x.BuscaLogradouro(It.IsAny<string>())).Returns(Task.FromResult(new ViewModels.ViaCepResponse() { Cep = "12345-678" }));

            var viewModel = new ConsultaCepViewModel()
            {
                Cep = "12345-678"
            };

            // Act
            var result = await _controller.Consultar(viewModel, "Consultar");

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var modelResult = Assert.IsType<ConsultaCepViewModel>(viewResult.Model);
            Assert.Equal(viewModel.Cep, modelResult.Cep);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Collection(_controller.ModelState.Values.SelectMany(a => a.Errors), item => Assert.Equal("CEP já cadastrado em nossa base de dados!", item.ErrorMessage));
        }

        [Fact]
        public async Task Consultar_Post_CadastrarCep()
        {
            // Arrange
            var resultCep = new ViaCepResponse()
            {
                Cep = "12345-678",
                Logradouro = "Rua Teste",
                Bairro = "Bairro Teste",
                Complemento = "Complemento Teste",
                DDD = "000",
                Uf = "UF",
                Unidade = "Unidade Teste",
                Gia = "123",
                Ibge = "123456",
                Localidade = "Localidade Teste",
                Siafi = "1234",
            };

            _viaCepService.Setup(x => x.BuscaLogradouro(It.IsAny<string>())).Returns(Task.FromResult(resultCep));

            var viewModel = new ConsultaCepViewModel()
            {
                Cep = "12345-678"
            };

            // Act
            var result = await _controller.Consultar(viewModel, "CadastrarNovoCep");

            // Assert
            Assert.NotNull(result);

            var viewResult = Assert.IsType<ViewResult>(result);
            var modelResult = Assert.IsType<ConsultaCepViewModel>(viewResult.Model);
            Assert.Equal(viewModel.Cep, modelResult.Cep);

            var logradouros = await _db.Logradouros.ToListAsync();

            Assert.Collection(logradouros, item =>
            {
                Assert.Equal("12345-678", item.Cep);
                Assert.Equal("Rua Teste", item.Descricao);
                Assert.Equal("Complemento Teste", item.Complemento);
                Assert.Equal("000", item.DDD);
                Assert.Equal("123", item.Gia);
                Assert.Equal("123456", item.Ibge);
                Assert.Equal("Localidade Teste", item.Localidade);
                Assert.Equal("1234", item.Siafi);
                Assert.Equal("Unidade Teste", item.Unidade);
                Assert.Equal("Bairro Teste", item.Bairro);
                Assert.Equal("UF", item.Uf);
            });

            Assert.Equal("CEP cadastrado na base de dados!!", _controller.TempData["success"]);
        }
    }
}
