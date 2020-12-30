using System;
using System.Collections.Generic;
using Elgin.Portal.Services.Implementation;

namespace Elgin.Portal.Services.Model
{
    public class PecaReposicao
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; }
        public string CodigoPecaReposicao { get; set; }
        public string Referencia { get; set; }
        public decimal Preco { get; set; }
        public Guid IdProduto { get; set; }
        public bool Ativo{ get; set; }
        public Arquivo Arquivo { get; set; }
    }
}
