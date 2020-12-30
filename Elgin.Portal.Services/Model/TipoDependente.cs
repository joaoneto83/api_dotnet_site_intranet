using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class TipoDependente
    {
        public Guid Id { get; set; }
        public string NomeTipoDependente { get; set; }
        public string CodigoTipoDependente { get; set; }
        public bool Ativo { get; set; }
    }
}
