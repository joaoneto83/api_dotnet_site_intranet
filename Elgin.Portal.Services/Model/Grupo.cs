using System;
using System.Collections.Generic;

namespace Elgin.Portal.Services.Model
{
    public class Grupo
    {
        public Guid Id { get; set; }
        public string NomeGrupo { get; set; }
        public string DataCriacao { get; set; }
        public bool Ativo { get; set; }
        public List<Usuario> Pessoas { get; set; }

    }
}
