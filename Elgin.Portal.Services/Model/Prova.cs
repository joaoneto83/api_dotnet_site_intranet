using System;
using System.Collections.Generic;

namespace Elgin.Portal.Services.Model
{
    public class Prova
    {
        public Guid Id { get; set; }
        public Guid IdUltimaAlteracao { get; set; }
        public Guid? IdLinha { get; set; }
        public Guid? IdProduto { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string TipoProva { get; set; }
        public int QtdQuestoes { get; set; }
        public int QtdAcertos { get; set; }
        public decimal Nota { get; set; }
        public int TempoRestante { get; set; }
        public string DataProva { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public bool Ativo { get; set; }
        public string NomeProduto { get; set; }
        public bool ProvaAtiva { get; set; }
        public bool UsuarioCorreto { get; set; }
        public bool ProvaRealizada { get; set; }
        public List<Questao> Questoes { get; set; }
    }
}
