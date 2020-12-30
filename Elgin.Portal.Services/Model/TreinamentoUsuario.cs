using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class TreinamentoUsuario
    {
        public Usuario Usuario { get; set; }
        public List<ProvaUsuario> ProvasUsuario { get; set; }
        public List<Grupo> GruposUsuario { get; set; }
        public ProvasGrafico ProvasGrafico { get; set; }
        public string GruposUsuarioTexto
        {
            get
            {
                if (GruposUsuario == null || GruposUsuario.Count == 0) return "";

                return string.Join(", ", GruposUsuario.Select(x => x.NomeGrupo));
            }
        }
    }
}
