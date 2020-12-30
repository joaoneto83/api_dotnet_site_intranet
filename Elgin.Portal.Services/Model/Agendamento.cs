using System;

namespace Elgin.Portal.Services.Model
{
    public class Agendamento
    {
        public Guid Id { get; set; }
        public Guid IdProva { get; set; }
        public Guid IdUsuarioAlteracao { get; set; }
        public Guid? IdGrupo { get; set; }
        public Guid? IdPessoa { get; set; }
        public string NomeUsuario { get; set; }
        public string NomeGrupo { get; set; }
        public bool Ativo { get; set; }
        public int QtdQuestoes { get; set; }
        public int Duracao { get; set; }
        public string Descricao { get; set; }
        public string DataDe { get; set; }
        public string DataAte { get; set; }
        public string NomeGrupoPessoa { get; set; }
        public string TituloProva { get; set; }
        public bool ProvaStartada { get; set; }
        public bool ProvaAtiva { get; set; }
        public Prova Prova { get; set; }
        
    }
}
