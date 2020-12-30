using System;

namespace Elgin.Portal.Services.Model
{
    public class Alternativa
    {
        public Guid Id { get; set; }
        public Guid IdQuestao { get; set; }   
        public Guid IdAlternativaQuestaoAgendamentoUsuario { get; set; }
        public string Descricao { get; set; }
        public int Ordem { get; set; }
        public bool Selecionada { get; set; }
        public bool Correta { get; set; }
        public bool Ativo { get; set; }
    }
}
