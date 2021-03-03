using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carflix.ViewModels
{
    public class ConsultaCepViewModel
    {
        public string Cep { get; set; }

        public ViaCepResponse Resposta { get; set; }
    }
}
