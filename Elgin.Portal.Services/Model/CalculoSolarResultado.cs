using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class CalculoSolarResultado
    {
        public string condicaoSolar { get; set; }
        public decimal estimativaArea { get; set; }
        public decimal economiaEnergia { get; set; }
        public decimal economiaFinanceira { get; set; }
        public int qtdIntegradores { get; set; }
        public decimal kitTotal
        {
            get
            {
                return kit1 + kit2;
            }
        }
        public decimal kit1 { get; set; }
        public decimal kit2 { get; set; }
        public decimal kitNecessario { get; set; }

        public string nome { get; set; }
        public string email { get; set; }

        public CalculoSolar calculo { get; set; }
    }
}
