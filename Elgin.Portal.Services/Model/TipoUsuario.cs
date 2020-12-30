using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class TipoUsuario
    {
        public Guid Id { get; set; }
        public string NomeTipoUsuario { get; set; }
        public string CodigoTipoUsuario { get; set; }
        public bool Ativo { get; set; }
    }
}
