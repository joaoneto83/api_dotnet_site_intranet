using System;
using System.Collections.Generic;
using Elgin.Portal.Services.Implementation;

namespace Elgin.Portal.Services.Model
{
    public class PalavraChave
    {
        public Guid Id { get; set; }
        public string Valor { get; set; }
        public Guid IdProduto { get; set; }
    }
}
