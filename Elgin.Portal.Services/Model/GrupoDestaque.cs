using System;
using System.Collections.Generic;

namespace Elgin.Portal.Services.Model
{
    public class GrupoDestaque
    {
        public Guid Id { get; set; }
        public string NomeGrupoDestaque { get; set; }
        public string CodigoGrupoDestaque { get; set; }
        public bool Ativo { get; set; }
        public List<Produto> Produtos { get; set; }
        public string Link { get; set; }
    }
}
