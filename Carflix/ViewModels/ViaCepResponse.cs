using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Carflix.ViewModels
{
    public class ViaCepResponse
    {
        [JsonProperty("cep")]
        [Display(Name = "CEP")]
        public string Cep { get; set; }

        [JsonProperty("logradouro")]
        public string Logradouro { get; set; }

        [JsonProperty("complemento")]
        public string Complemento { get; set; }

        [JsonProperty("bairro")]
        public string Bairro { get; set; }

        [JsonProperty("localidade")]
        [Display(Name = "Cidade")]
        public string Localidade { get; set; }

        [JsonProperty("uf")]
        [Display(Name = "UF")]
        public string Uf { get; set; }

        [JsonProperty("unidade")]
        [Display(Name = "Estado")]
        public string Unidade { get; set; }

        [JsonProperty("ibge")]
        [Display(Name = "Código IBGE")]
        public string Ibge { get; set; }

        [JsonProperty("gia")]
        public string Gia { get; set; }

        [JsonProperty("ddd")]
        public string DDD { get; set; }

        [JsonProperty("siafi")]
        public string Siafi { get; set; }

        [JsonProperty("erro")]
        public bool Erro { get; set; }
    }
}
