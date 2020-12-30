using System;
using System.Collections.Generic;

namespace Elgin.Portal.Services.Model
{
    public class AssistenciaVinculada
    {
        public Guid Id { get; set; }
        public string Documento { get; set; }
        public bool Ativo { get; set; }
        public List<SubLinha> SubLinhas { get; set; }
    }
}
