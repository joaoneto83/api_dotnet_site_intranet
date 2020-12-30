using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class SecaoModeloGrupo
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public bool Ativo { get; set; }
        public List<SecaoModeloGrupoProduto> IdProdutos { get; set; }
        

    }

    public class SecaoModeloGrupoProduto
    {
        public string Id { get; set; }
        public bool Ativo { get; set; }
        
    }
}
