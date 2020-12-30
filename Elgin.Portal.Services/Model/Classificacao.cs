using System;
using System.Collections.Generic;

namespace Elgin.Portal.Services.Implementation
{
    public class Classificacao
    {
        public Classificacao()
        {
            Filhos = new List<Classificacao>();
        }

        public Guid Id { get; set; }
        public Guid IdProduto { get; set; }
        public string NomeClassificacao { get; set; }
        public bool PossuiFiltroPilha { get; set; }
        public string CaminhoImagem { get; set; }
        public Guid? IdClassificacaoSuperior { get; set; }
        public Guid IdSubLinha { get; set; }
        public bool Selecionado { get; set; }
        public bool Comparativo { get; set; }
        public int Ordem { get; set; }
        public bool Ativo { get; set; }
        public bool Expansivo { get; set; }
        public List<Classificacao> Filhos { get; set; }
    }
}
