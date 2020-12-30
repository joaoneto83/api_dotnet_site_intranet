using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class SolarSimulacoes
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public int IdCidade { get; set; }
        public string UF { get; set; }
        public string Onde { get; set; }
        public string FormatoFatura { get; set; }
        public string FaturaMensal { get; set; }
        public string MediaMensal { get; set; }
        public string CondicaoSolar { get; set; }
        public string EstimativaArea { get; set; }
        public string Economia { get; set; }
        public string EcominaFinanceira { get; set; }
        public string Kit1 { get; set; }
        public string Kit2 { get; set; }
        public string KitTotal { get; set; }
        public string KitNecessario { get; set; }
        public string Data { get; set; }
    }
}
