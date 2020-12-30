using System;

namespace Elgin.Portal.Services.Model
{
    public class Resultado
    {
        public Guid? IdProva { get; set; }
        public Guid? IdGrupo { get; set; }
        public string DataDe { get; set; }
        public string DataAte { get; set; }
        public int QtdDisponibilizados { get; set; }
        public int PercentRealizados { get; set; }
        public int PercentAproveitamento { get; set; }
        public int TotalProvas { get; set; }
    }
}
