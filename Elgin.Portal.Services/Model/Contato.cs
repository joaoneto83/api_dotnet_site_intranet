using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Contato
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Mensagem { get; set; }
        public string IdLinha { get; set; }
        public string Tipo { get; set; }
        public string Assunto { get; set; }
        public string EmpresaAreaAtuacao { get; set; }
    }
}
