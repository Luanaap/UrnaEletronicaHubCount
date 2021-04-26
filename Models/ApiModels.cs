using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrnaEletronica.Models
{
    public class ApiModels
    {
        public class Voto
        {
            public int ID { get; set; }
            public int Candidato { get; set; }
            public DateTime Data { get; set; }
        }

        public class Candidato
        {
            public int ID { get; set; }
            public string NomeCompleto { get; set; }
            public string Vice { get; set; }
            public DateTime Inscricao { get; set; }
            public int Legenda { get; set; }
        }
    }
}
