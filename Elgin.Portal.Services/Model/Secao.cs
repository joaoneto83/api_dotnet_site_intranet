using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Secao
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public int QtdImagens { get; set; }
        public bool ExibeTexto1 { get; set; }
        public bool ExibeTexto2 { get; set; }
        public bool ExibeTexto3 { get; set; }
        public bool ExibeCodigoVideo { get; set; }
        public bool ExibeCodigoVideo2 { get; set; }
        public bool ExibeCodigoVideo3 { get; set; }
        public bool ExibeCodigoVideo4 { get; set; }
        public bool ExibeAparelhoIdeal { get; set; }
    }
}
