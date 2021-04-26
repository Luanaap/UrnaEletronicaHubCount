using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static UrnaEletronica.Models.ApiModels;

namespace UrnaEletronica.Data
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options)
           : base(options)
        { }

        public DbSet<Candidato> Candidatos { get; set; }
        public DbSet<Voto> Votos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=TesteErsonDB;Trusted_Connection=True;");
        }
    }

}

