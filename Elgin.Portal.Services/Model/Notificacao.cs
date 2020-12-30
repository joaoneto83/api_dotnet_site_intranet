using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Notificacao
    {
        public Guid Id { get; set; }
        public Guid IdUsuario { get; set; }
        public string Tipo { get; set; }
        public string Descricao { get; set; }
        public string Link { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime? DataVisualizacao { get; set; }
    }
}
