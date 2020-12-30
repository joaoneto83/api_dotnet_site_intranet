using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class ProvasGrafico
    {
        public int ProvasTotal { get; set; }
        public int ProvasRealizadas { get; set; }
        public int ProvasRealizadasPercent { get
            {
                if (ProvasTotal == 0) return 0;

                decimal calculo = ((decimal)ProvasRealizadas / (decimal)ProvasTotal) * 100;

                return (int)calculo;
            }
        }
        public int Aproveitamento { get; set; }
    }
}
