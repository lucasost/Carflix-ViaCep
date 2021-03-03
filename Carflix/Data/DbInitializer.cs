using Carflix.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Carflix.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CarflixContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Logradouros.Any())
            {
                return;   // DB has been seeded
            }

            var logradouros = new List<Logradouro>() {
            new Logradouro(){
                LogradouroId = Guid.NewGuid(),
                Bairro = "BairroTeste",
                Complemento = "ComplementoTeste",
                Cep = "00000000",
                DDD = "000",
                Descricao = "Logradouro Teste",
                Gia ="",
                Ibge = "123123123",
                Uf="TS",
                Localidade = "Cidade Teste",
                Siafi ="123123",
                Unidade = ""
               }
            };

            context.Logradouros.AddRange(logradouros);
            context.SaveChanges();
        }
    }
}
