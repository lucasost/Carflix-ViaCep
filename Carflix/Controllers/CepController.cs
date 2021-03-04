using Carflix.Extensions;
using Carflix.Services;
using Carflix.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Carflix.Controllers
{
    public class CepController : BaseController
    {
        private readonly CarflixContext context;

        private readonly IViaCepApiService viaCepApiService;

        public CepController(CarflixContext context, IViaCepApiService viaCepApiService)
        {
            this.context = context;
            this.viaCepApiService = viaCepApiService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new ConsultaCepViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Consultar(ConsultaCepViewModel viewModel, string value = "")
        {
            viewModel.Resposta = await this.viaCepApiService.BuscaLogradouro(viewModel.Cep);

            if (viewModel.Resposta.Erro)
            {
                ModelState.AddModelError("", "CEP inválido ou não encontrado");
            }
            else
            {
                var existeCepCadastrado = await context.Logradouros.AnyAsync(a => a.Cep.Equals(viewModel.Cep));
                if (existeCepCadastrado)
                {
                    ModelState.AddModelError("", "CEP já cadastrado em nossa base de dados!");
                }
            }

            if (ModelState.IsValid)
            {
                if (value.Equals("CadastrarNovoCep"))
                {
                    context.Logradouros.Add(new Models.Logradouro()
                    {
                        LogradouroId = Guid.NewGuid(),
                        Cep = viewModel.Resposta.Cep,
                        Bairro = viewModel.Resposta.Bairro,
                        Complemento = viewModel.Resposta.Complemento,
                        Gia = viewModel.Resposta.Gia,
                        Ibge = viewModel.Resposta.Ibge,
                        Uf = viewModel.Resposta.Uf,
                        Unidade = viewModel.Resposta.Unidade,
                        Descricao = viewModel.Resposta.Logradouro,
                        Localidade = viewModel.Resposta.Localidade,
                        DDD = viewModel.Resposta.DDD,
                        Siafi = viewModel.Resposta.Siafi
                    });
                    await context.SaveChangesAsync();

                    TempData["success"] = "CEP cadastrado na base de dados!!";
                }

                if (Request?.IsAjaxRequest() ?? false)
                    return PartialView("_CepResultado", viewModel.Resposta);
            }

            if (Request?.IsAjaxRequest() ?? false)
                return PartialView("_PesquisaCep", viewModel);

            return View("Indice", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CadastrarCep(ViaCepResponse cepConsultado)
        {
            var existeCepCadastrado = await context.Logradouros.AnyAsync(a => a.Cep.Equals(cepConsultado.Cep));

            if (existeCepCadastrado)
            {
                ModelState.AddModelError("", "CEP já cadastrado em nossa base de dados!");
            }

            if (string.IsNullOrWhiteSpace(cepConsultado.Logradouro))
            {
                ModelState.AddModelError("", "CEP não pode inserido.");
            }

            if (ModelState.IsValid)
            {
                context.Logradouros.Add(new Models.Logradouro()
                {
                    LogradouroId = Guid.NewGuid(),
                    Cep = cepConsultado.Cep,
                    Bairro = cepConsultado.Bairro,
                    Complemento = cepConsultado.Complemento,
                    Gia = cepConsultado.Gia,
                    Ibge = cepConsultado.Ibge,
                    Uf = cepConsultado.Uf,
                    Unidade = cepConsultado.Unidade,
                    Descricao = cepConsultado.Logradouro,
                    Localidade = cepConsultado.Localidade,
                    Siafi = cepConsultado.Siafi,
                    DDD = cepConsultado.DDD,
                });

                await context.SaveChangesAsync();

                TempData["success"] = "CEP cadastrado na base de dados!!";
            }

            return PartialView("_PesquisaCep", null);
        }

        [HttpGet]
        public async Task<ActionResult> ListarCepCadastrado()
        {
            var logradouros = await context.Logradouros.ToListAsync();

            return View(logradouros);
        }

    }
}
