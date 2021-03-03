using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Carflix.Models
{
    [Table("Logradouros")]
    public class Logradouro
    {
        [Key]
        public Guid LogradouroId { get; set; }

        public string Cep { get; set; }

        public string Descricao { get; set; }

        public string Complemento { get; set; }

        public string Bairro { get; set; }

        public string Localidade { get; set; }

        public string Uf { get; set; }

        public string Unidade { get; set; }

        public string Ibge { get; set; }

        public string Gia { get; set; }

        public string DDD { get; set; }

        public string Siafi { get; set; }
    }
}
