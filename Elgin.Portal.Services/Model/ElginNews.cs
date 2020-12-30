using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class ElginNews
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Ano { get; set; }
        public int Numero { get; set; }
        public string Imagem { get; set; }
        public string Arquivo { get; set; }
        public bool Ativo { get; set; }
        public DateTime Data { get; set; }
    }
}
