using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class ResponsePastaTabelaPreco
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Ativo { get; set; }
        public Guid? IdPai { get; set; }
        public List<TabelaPreco> TabelaPrecos { get; set; }
        public List<PastaTabelaPreco> Pastas { get; set; }
    }
}
