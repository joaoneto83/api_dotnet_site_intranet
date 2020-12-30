using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Funcionalidade
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public Guid IdMenu { get; set; }
        public bool Selecionado { get; set; }
    }
}
