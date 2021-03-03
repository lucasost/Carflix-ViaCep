using Carflix.ViewModels;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carflix.Services
{
    public interface IViaCepApiService
    {
        [Get("/ws/{cep}/json/")]
        Task<ViaCepResponse> BuscaLogradouro(string cep);
    }
}
