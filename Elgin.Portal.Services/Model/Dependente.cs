using System;

namespace Elgin.Portal.Services.Model
{
    public class Dependente
    {
        public Guid Id { get; set; }
        public string NomeDependente { get; set; }
        public string Celular { get; set; }
        public string DataNascimento { get; set; }
        public Guid IdUsuario { get; set; }
        public Guid IdTipoDependente { get; set; }
    }

}
