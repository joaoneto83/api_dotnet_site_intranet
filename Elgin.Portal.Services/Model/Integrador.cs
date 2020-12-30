using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Integrador
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string TelefoneFixo { get; set; }
        public string TelefoneMovel { get; set; }
        public string Email { get; set; }
        public string Site { get; set; }
        public string NomePais { get; set; }
        public string NomeEstado { get; set; }
        public string NomeCidade { get; set; }
        public int IdPais { get; set; }
        public string Uf { get; set; }
        public int? IdCidade { get; set; }
        public bool Ativo { get; set; }
        public int IdSegmento { get; set; }
        public string NomeSegmento { get; set; }
    }
}
