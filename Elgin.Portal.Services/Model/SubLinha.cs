using Elgin.Portal.Services.Implementation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class SubLinha
    {
        public Guid Id { get; set; }
        public string NomeSubLinha { get; set; }
        public string CodigoSubLinha { get; set; }
        public Guid IdLinha { get; set; }
        public Guid IdArquivo { get; set; }
        public bool Ativo { get; set; }
        public string CaminhoImagem { get; set; }
        public string NomeArquivo { get; set; }
        public bool MostraAparelhoIdeal { get; set; }
        public int Ordem { get; set; }
        public bool PossuiFiltroPilha { get; set; }
        public bool MostraLink { get; set; }
        public bool MostraRota { get; set; }
        public string TextoUrl { get; set; }
        public string TextoBotao { get; set; }
        public string TextoInformativo { get; set; }

        public Arquivo Arquivo { get; set; }
        public Arquivo Banner { get; set; }
        public Arquivo Banner2 { get; set; }
        public virtual List<Produto> Produtos { get; set; }
        public virtual List<Classificacao> Classificacoes { get; set; }
        public virtual List<EspecificacaoTecnica> Especificacoes { get; set; }
    }
}
