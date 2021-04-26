using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrnaEletronica.Models
{
    public class VotoViewModels
    {
        public class VotoViewModel
        {
            public int Candidato { get; set; }
        }
        public class ResultadosViewModel
        {
            public string Candidato { get; set; }
            public string Vice { get; set; }
            public int Legenda { get; set; }
            public int Votos { get; set; }
        }
    }
}
