using System;

namespace Elgin.Portal.Services.Model
{
    public class Caracteristica
    {
        public Guid Id {get; set; }
        public string DescricaoCaracteristica { get; set; }
        public Guid IdProduto { get; set; }
        public bool CaracteristicaPrincipal { get; set; }
        public Guid? IdIcone { get; set; }
        public string UrlIcone { get; set; }
        public int Ordem { get; set; }
    }
}