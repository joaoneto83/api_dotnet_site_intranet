using System;
using System.Collections.Generic;

namespace Elgin.Portal.Services.Model
{
    public class TabelaPreco
    {
        public Guid Id { get; set; }
        public string NomeTabelaPreco { get; set; }
        public bool Ativo { get; set; }

        public Guid IdPastaTabelaPreco { get; set; }
        public virtual List<Arquivo> Arquivos { get; set; }
    }
}
