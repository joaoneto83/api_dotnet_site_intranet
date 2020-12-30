using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Assistencia
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Rua { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Complemento { get; set; }
        public string Cep { get; set; }
        public string Telefone { get; set; }
        public string Telefone_2 { get; set; }
        public string Telefone_3 { get; set; }
        public string Telefone_4 { get; set; }
        public string Email { get; set; }
        public int IdCidade { get; set; }
        public int IdEstado { get; set; }
        public Guid IdLinha { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string NomeEstado { get; set; }
        public string NomeCidade { get; set; }
        public bool Ativo { get; set; }
    }
}
