using Carflix.Extensions;
using Carflix.Services;
using Carflix.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Refit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Carflix.Controllers
{
    public class ConsultaCepController : BaseController
    {
        private CarflixContext context;

        public ConsultaCepController(CarflixContext context)
        {
            this.context = context;
        }

        // GET: ConsultaCepController
        [HttpGet]
        public ActionResult Index()
        {
            return View(new ConsultaCepViewModel());
        }

        [HttpGet]
        public async Task<ActionResult> Consultar(ConsultaCepViewModel viewModel)
        {
            var cepClient = RestService.For<IViaCepApiService>("http://viacep.com.br");

            var logradouro = await cepClient.BuscaLogradouro(viewModel.Cep);

            if (string.IsNullOrWhiteSpace(logradouro.Erro))
            {
                viewModel.Resposta = new ViaCepResponse();
                viewModel.Resposta.Cep = logradouro.Cep;
                viewModel.Resposta.Bairro = logradouro.Bairro;
                viewModel.Resposta.Complemento = logradouro.Complemento;
                viewModel.Resposta.Gia = logradouro.Gia;
                viewModel.Resposta.Ibge = logradouro.Ibge;
                viewModel.Resposta.Uf = logradouro.Uf;
                viewModel.Resposta.Unidade = logradouro.Unidade;
                viewModel.Resposta.Logradouro = logradouro.Logradouro;
                viewModel.Resposta.Localidade = logradouro.Localidade;
                viewModel.Resposta.Gia = logradouro.Gia;
                viewModel.Resposta.DDD = logradouro.DDD;
                viewModel.Resposta.Siafi = logradouro.Siafi;

                var existeCepCadastrado = await context.Logradouros.FirstOrDefaultAsync(a => a.Cep.Equals(logradouro.Cep));

                if (existeCepCadastrado == null)
                {
                    context.Logradouros.Add(new Models.Logradouro()
                    {
                        LogradouroId = Guid.NewGuid(),
                        Cep = logradouro.Cep,
                        Bairro = logradouro.Bairro,
                        Complemento = logradouro.Complemento,
                        Gia = logradouro.Gia,
                        Ibge = logradouro.Ibge,
                        Uf = logradouro.Uf,
                        Unidade = logradouro.Unidade,
                        Descricao = logradouro.Logradouro,
                        Localidade = logradouro.Localidade
                    });
                    context.SaveChanges();
                    TempData["success"] = "CEP cadastrado na base de dados!!";
                }
                else
                {
                    TempData["warning"] = "CEP já estava cadastrado em nossa base de dados!!";
                }
            }
            else
            {
                TempData["error"] = "CEP não encontrado!!";
            }

            if (Request?.IsAjaxRequest() ?? false)
                return PartialView("_CepResultado", viewModel.Resposta);

            return View("Indice", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CadastrarCep(ViaCepResponse cepConsultado)
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
                Localidade = cepConsultado.Localidade
            });

            context.SaveChanges();

            return View("Indice");
        }

        // GET: ConsultaCepController/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ConsultaCepController/Create

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ConsultaCepController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ConsultaCepController/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ConsultaCepController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ConsultaCepController/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ConsultaCepController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
