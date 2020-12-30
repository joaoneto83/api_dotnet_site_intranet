using System;

namespace Elgin.Portal.Services.Model
{
    public class SecaoProdutoIcone
    {
        public Guid Id {get; set; }
        public string DescricaoIcone { get; set; }
        public string SubDescricaoIcone { get; set; }
        public Guid IdSecaoProduto { get; set; }
        public Guid? IdIcone { get; set; }
        public string UrlIcone { get; set; }
        public int Ordem { get; set; }
    }
}