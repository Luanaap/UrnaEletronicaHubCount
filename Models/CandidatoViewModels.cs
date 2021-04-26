using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrnaEletronica.Models
{
    public class CandidatoViewModels
    {
        public class CandidatoViewModel
        {
            public string NomeCompleto { get; set; }
            public string Vice { get; set; }
            public DateTime Inscricao { get; set; }
            public int Legenda { get; set; }
        }
    }
}
