using System;

namespace Elgin.Portal.Services.Model
{
    public class ResponsavelSetor
    {
        public Guid Id { get; set; }
        public string Setor { get; set; }
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public bool Ativo { get; set; }
        public string[] Telefones {
            get { return Telefone?.Split('|'); }
        }
    }
}
