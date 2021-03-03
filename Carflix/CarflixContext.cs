using Carflix.Models;
using Microsoft.EntityFrameworkCore;

namespace Carflix
{
    public class CarflixContext : DbContext
    {
        public CarflixContext(DbContextOptions<CarflixContext> options)
            : base(options)
        {

        }

        public DbSet<Logradouro> Logradouros { get; set; }


    }
}
