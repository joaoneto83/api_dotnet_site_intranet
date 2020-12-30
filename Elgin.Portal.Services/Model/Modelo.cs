using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Modelo
    {
        public Guid Id { get; set; }
        public string CodigoModelo { get; set; }
        public string NomeModelo { get; set; }
        public bool Ativo{ get; set; }
        public decimal AparelhoIdealDe { get; set; }
        public decimal AparelhoIdealAte { get; set; }
        public Guid IdProduto { get; set; }
    }
}
