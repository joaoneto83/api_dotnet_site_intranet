using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Menu
    {
        public Menu()
        {
            Filhos = new List<Menu>();
            Funcionalidades = new List<Funcionalidade>();
        }

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public string Rota { get; set; }
        public int Ordem { get; set; }
        public bool Ativo { get; set; }
        public Guid IdMenuPai { get; set; }
        public virtual List<Funcionalidade> Funcionalidades { get; set; }
        public virtual List<Menu> Filhos { get; set; }
    }
}
