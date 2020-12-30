using System;

namespace Elgin.Portal.Services.Model
{
    public class Evento
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }
        public string DataDe { get; set; }
        public string DataAte { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public string Periodo {
            get { return dtDataDe == dtDataAte ? dtDataDe.ToString("dd/MM") : dtDataDe.ToString("dd/MM") + " a " + dtDataAte.ToString("dd/MM"); }
        }
        public DateTime dtDataDe { get; set; }
        public DateTime dtDataAte { get; set; }
    }
}
