using System;

namespace Elgin.Portal.Services.Model
{
    public class AgendamentoUsuario
    {
        public Guid Id { get; set; }
        public Guid IdAgendamento { get; set; }
        public Guid IdUsuario { get; set; }
        public Guid IdProva { get; set; }
        public decimal Nota{ get; set; }
        public string InicioProva { get; set; }
        public string TerminoProva { get; set; }
        public DateTime? dtInicioProva { get; set; }
        public DateTime? dtTerminoProva { get; set; }
        public DateTime DataDe { get; set; }
        public DateTime DataAte { get; set; }
        public int QtdQuestoes { get; set; }
        public int Duracao { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public bool ProvaIniciada
        {
            get
            {
                return dtInicioProva.HasValue;
            }
        }

        public bool ProvaRealizada
        {
            get
            {
                return dtTerminoProva.HasValue;
            }
        }

        public bool PodeIniciarProva
        {
            get
            {
                return DataDe <= DateTime.Today;
            }
        }

        public bool ProvaFinalizada
        {
            get
            {
                return DateTime.Today > DataAte;
            }
        }
    }
}
