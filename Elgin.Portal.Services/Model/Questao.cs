using System;
using System.Collections.Generic;

namespace Elgin.Portal.Services.Model
{
    public class Questao
    {
        public Guid Id { get; set; }
        public Guid IdProva { get; set; }   
        public Guid? IdAgendamentoUsuario { get; set; }
        public Guid? IdQuestaoAgendamentoUsuario { get; set; }
        public string Descricao { get; set; }
        public int Ordem { get; set; }
        public bool Ativo { get; set; }
        public List<Alternativa> Alternativas { get; set; }
    }
}
