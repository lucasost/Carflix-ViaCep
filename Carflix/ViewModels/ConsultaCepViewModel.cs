using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Carflix.ViewModels
{
    public class ConsultaCepViewModel
    {
        [Required(ErrorMessage = "Informe o CEP.")]
        [Display(Name = "CEP")]
        public string Cep { get; set; }

        public ViaCepResponse Resposta { get; set; }
    }
}
