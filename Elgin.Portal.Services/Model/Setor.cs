using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Setor
    {
        public Guid Id { get; set; }
        public string NomeSetor { get; set; }
        public string CodigoSetor { get; set; }
        public bool Ativo { get; set; }
    }
}
