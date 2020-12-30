using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class TipoPagamento
    {
        public Guid Id { get; set; }
        public string CodigoTipoPagamento { get; set; }
        public string NomeTipoPagamento { get; set; }
    }
}
