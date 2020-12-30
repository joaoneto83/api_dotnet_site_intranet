using System;

namespace Elgin.Portal.Services.Model
{
    public class AgendaTreinamento
    {
        public Guid Id { get; set; }
        public string Empresa { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string DataDe { get; set; }
        public string DataAte { get; set; }
        
    }
}
