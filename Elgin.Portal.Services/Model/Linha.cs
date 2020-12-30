using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Linha
    {
        public Guid Id { get; set; }
        public string NomeLinha { get; set; }
        public string CodigoLinha { get; set; }
        public string Cor1 { get; set; }
        public string Cor2 { get; set; }
        public string CorTitulo { get; set; }
        public int Ordem { get; set; }
        public bool Ativo { get; set; }
        public virtual List<SubLinha> SubLinhas { get;  set; }
        public List<Arquivo> Arquivos { get; set; }
    }
}
