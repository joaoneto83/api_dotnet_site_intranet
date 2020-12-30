using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public Guid IdTipoPagamento { get; set; }
        public string CodigoTipoPagamento { get; set; }
        public int NumeroPedido { get; set; }
        public decimal ValorTotal { get; set; }
        public string NomeCompleto { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Cep { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Bairro { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }
        public bool IsPessoaFisica { get; set; }
        public string RazaoSocial { get; set; }
        public string Cnpj { get; set; }
        public string InscricaoEstadual { get; set; }
        public TipoPagamento TipoPagamento { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}
