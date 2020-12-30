using System;
using System.Collections.Generic;

namespace Elgin.Portal.Services.Model
{
    public class TreinamentoMerchan
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public Guid? IdPastaTreinamentoMerchan { get; set; }
        public virtual List<Arquivo> Arquivos { get; set; }
    }
}
