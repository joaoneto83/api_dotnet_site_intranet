using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class SolarIntegrador
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string CNPJ { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Site { get; set; }
        public string NomeEstado { get; set; }
        public string NomeCidade { get; set; }
        public string UF { get; set; }
        public int? IdCidade { get; set; }
        public bool Ativo { get; set; }
    }
}
