using Carflix.Controllers;
using Carflix.Models;
using Carflix.Services;
using Carflix.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
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
            Assert.Equal("Indice", viewResult.ViewName);
            var modelResult = Assert.IsType<ConsultaCepViewModel>(viewResult.Model);
            Assert.Equal(viewModel.Cep, modelResult.Cep);
            Assert.Equal("CEP cadastrado na base de dados!!", _controller.TempData["success"]);
        }

        [Fact]
        public async Task ListarCepCadastrado_GET_CadastrarCep()
        {
            // Arrange
            _db.Logradouros.Add(new Models.Logradouro()
            {
                Cep = "12345-678"
            });
            _db.Logradouros.Add(new Models.Logradouro()
            {
                Cep = "45678-789"
            });
            _db.Logradouros.Add(new Models.Logradouro()
            {
                Cep = "00000-000"
            });
            await _db.SaveChangesAsync();

            // Act
            var result = await _controller.ListarCepCadastrado();

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var modelResult = Assert.IsType<List<Logradouro>>(viewResult.Model);

            var logradouros = modelResult;

            Assert.Collection(logradouros,
                item => Assert.Equal("12345-678", item.Cep),
                item => Assert.Equal("45678-789", item.Cep),
                item => Assert.Equal("00000-000", item.Cep)
         );
        }

        [Fact]
        public async Task CadastrarCep_Post_QuandoNaoHaLogradouroNaoPodeInserir()
        {
            // Arrange
            var viewModel = new ViaCepResponse()
            {
                Cep = "00000-000"
            };

            // Act
            var result = await _controller.CadastrarCep(viewModel);

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("_PesquisaCep", viewResult.ViewName);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Collection(_controller.ModelState.Values.SelectMany(a => a.Errors), item => Assert.Equal("CEP não pode inserido.", item.ErrorMessage));
        }

        [Fact]
        public async Task CadastrarCep_Post_QuandoCepJaEstaCadastradoDeveSerExibidaAoUsuario()
        {
            // Arrange
            _db.Logradouros.Add(new Models.Logradouro()
            {
                Cep = "12345-678"
            });
            await _db.SaveChangesAsync();

            var viewModel = new ViaCepResponse()
            {
                Cep = "12345-678",
                Logradouro = "Teste logradouro"
            };

            // Act
            var result = await _controller.CadastrarCep(viewModel);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PartialViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.Collection(_controller.ModelState.Values.SelectMany(a => a.Errors), item => Assert.Equal("CEP já cadastrado em nossa base de dados!", item.ErrorMessage));
        }

        [Fact]
        public async Task CadastrarCep_Post_CadastrarCep()
        {
            // Arrange
            var viewModel = new ViaCepResponse()
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

            // Act
            var result = await _controller.CadastrarCep(viewModel);

            // Assert
            Assert.NotNull(result);
            var viewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("CEP cadastrado na base de dados!!", _controller.TempData["success"]);

            var logradouro = await _db.Logradouros.FirstOrDefaultAsync();

            Assert.Equal(viewModel.Bairro, logradouro.Bairro);
            Assert.Equal(viewModel.Cep, logradouro.Cep);
            Assert.Equal(viewModel.Complemento, logradouro.Complemento);
            Assert.Equal(viewModel.DDD, logradouro.DDD);
            Assert.Equal(viewModel.Logradouro, logradouro.Descricao);
            Assert.Equal(viewModel.Gia, logradouro.Gia);
            Assert.Equal(viewModel.Ibge, logradouro.Ibge);
            Assert.Equal(viewModel.Siafi, logradouro.Siafi);
            Assert.Equal(viewModel.Uf, logradouro.Uf);
            Assert.Equal(viewModel.Unidade, logradouro.Unidade);
            Assert.Equal(viewModel.Localidade, logradouro.Localidade);
        }
    }
}
