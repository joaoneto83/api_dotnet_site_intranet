using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class PedidoProduto
    {
        public Guid IdPedido { get; set; }
        public Guid IdProduto { get; set; }
        public int QtdProduto { get; set; }
        public decimal ValorTotalProduto { get; set; }
    }
}
