using Carflix.Extensions;
using Carflix.Services;
using Carflix.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carflix.Controllers
{
    public class ConsultaCepController : Controller
    {
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

            if (logradouro != null)
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
            }

            if (Request?.IsAjaxRequest() ?? false)
                return PartialView("_CepResultado", viewModel.Resposta);

            return View("Indice", viewModel);
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
