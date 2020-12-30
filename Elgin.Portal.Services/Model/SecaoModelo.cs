using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class SecaoModelo
    {
        public Guid Id { get; set; }
        public Guid IdSecao { get; set; }
        public Guid IdSecaoModeloGrupo { get; set; }
        public string Descricao { get; set; }
        public int QtdImagens { get; set; }
        public int QtdIcones { get; set; }
        public bool ExibeTexto1 { get; set; }
        public bool ExibeTexto2 { get; set; }
        public bool ExibeTexto3 { get; set; }
        public bool ExibeCodigoVideo { get; set; }
        public bool ExibeCodigoVideo2 { get; set; }
        public bool ExibeCodigoVideo3 { get; set; }

        public bool ExibeCodigoVideo4 { get; set; }
        public bool ExibeAparelhoIdeal { get; set; }
        public string DescricaoModelo { get; set; }
        public string Texto1 { get; set; }
        public string Texto2 { get; set; }
        public string Texto3 { get; set; }
        public int Ordem { get; set; }
        public bool Ativo { get; set; }
        public string CodigoSecao { get; set; }
        public string CodigoVideo { get; set; }
        public string CodigoVideo2 { get; set; }
        public string CodigoVideo3 { get; set; }
        public string CodigoVideo4 { get; set; }
        public bool AparelhoIdeal { get; set; }

        public virtual List<Arquivo> Arquivos { get; set; }
        public virtual List<SecaoModeloIcone> Icones { get; set; }
    }
}
