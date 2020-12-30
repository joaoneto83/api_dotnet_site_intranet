using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Catalogo
    {
        public Guid IdLinha { get; set; }
        public Guid IdArquivoLinha { get; set; }
        public string UrlCatalogoLinha { get; set; }
        public string NomeArquivoLinha { get; set; }
        public string NomeLinha { get; set; }
        public string Cor1 { get; set; }
        public string Cor2 { get; set; }
        public string CorTitulo { get; set; }
        public List<SubLinha> SubLinhas { get; set; }
        public List<Produto> Produtos { get; set; }
        public List<Arquivo> Arquivos { get; set; }
    }
}
