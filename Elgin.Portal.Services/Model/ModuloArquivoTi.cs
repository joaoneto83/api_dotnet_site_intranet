using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class ModuloArquivoTi
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Secao { get; set; }
        public bool Ativo { get; set; }
        public virtual List<Arquivo> Arquivos { get; set; }
    }
}
