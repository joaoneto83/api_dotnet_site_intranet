using System;
using System.Collections.Generic;

namespace Elgin.Portal.Services.Model
{
    public class ProvaUsuario
    {
        public Guid Id { get; set; }
        public Guid IdAgendamento { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime InicioProva { get; set; }
        public DateTime? TerminoProva { get; set; }
        public DateTime DataDe { get; set; }
        public DateTime DataAte { get; set; }
        public decimal Nota { get; set; }
        public int QtdAcertos { get; set; }
        public int QtdQuestoes { get; set; }
        public Guid IdProva { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataProva { get; set; }
        public bool Ativo { get; set; }
        public Guid IdLinha { get; set; }
        public Guid IdProduto { get; set; }
        public string NomeLinha { get; set; }
        public string NomeProduto { get; set; }
        public string NomeUSuario { get; set; }
        public bool ProvaRealizada
        {
            get
            {
                return TerminoProva.HasValue;
            }
        }
        public bool PeriodoAtivo
        {
            get
            {
                return (DateTime.Now >= DataDe) && (DateTime.Now < DataAte.AddDays(1));
            }
        }
    }
}
