using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Banner
    {
        public Guid id { get; set; }
        public string modulo { get; set; }
        public string componente { get; set; }
        public int posicao { get; set; }
        public string caminho { get; set; }
        public string caminho2 { get; set; }
        public string texto1 { get; set; }
        public string texto2 { get; set; }
        public string texto3 { get; set; }
        public string link { get; set; }
        public string cor { get; set; }
        public bool ativo { get; set; }
    }
}
