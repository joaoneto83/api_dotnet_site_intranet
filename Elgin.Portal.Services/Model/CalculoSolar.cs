using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class CalculoSolar
    {
        public string estado { get; set; }
        public int idCidade { get; set; }
        public string onde { get; set; }
        public decimal faturaMensal { get; set; }
        public decimal? mediaMensal { get; set; }

        public string cidadeTexto { get; set; }
        public string estadoTexto { get; set; }
    }
}
