using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class TipoArquivo
    {
        public Guid Id { get; set; }
        public string NomeTipoArquivo { get; set; }
        public string CodigoTipoArquivo { get; set; }
        public bool Ativo{ get; set; }
    }
}
