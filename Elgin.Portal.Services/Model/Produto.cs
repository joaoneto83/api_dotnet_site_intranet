using System;
using System.Collections.Generic;
using Elgin.Portal.Services.Implementation;

namespace Elgin.Portal.Services.Model
{
    public class Produto
    {
        public Guid Id { get; set; }
        public string NomeProduto { get; set; }
        public string CodigoLegado { get; set; }
        public string CodigoProduto { get; set; }
        public Guid IdSubLinha{ get; set; }
        public Guid IdLinha{ get; set; }
        public bool Ativo{ get; set; }
        public string NomeLinha { get; set; }
        public string NomeSublinha { get; set; }
        public string CodigoLinha { get; set; }
        public string CodigoSublinha { get; set; }
        public string ImagemUrl { get; set; }
        public decimal Preco { get; set; }
        public int QtdProduto { get; set; }
        public virtual List<Arquivo> Arquivos { get; set; }
        public virtual List<Caracteristica> Caracteristicas { get;set; }
        public virtual List<EspecificacaoTecnica> EspecificacoesTecnicas { get; set; }
        public virtual List<SecaoProduto> SecoesProduto { get; set; }
        public virtual List<SecaoModelo> SecoesModelo { get; set; }
        public virtual List<Classificacao> Classificacoes { get; set; }
        public virtual List<Modelo> Modelos { get; set; }
        public virtual List<PalavraChave> PalavrasChave { get; set; }
    }
}
