using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Arquivo
    {
        public Guid Id { get; set; }
        public Guid IdTipoArquivo { get; set; }
        public string NomeArquivo { get; set; }
        public string CodigoTipoArquivo { get; set; }
        public string Caminho { get; set; }
        public int Ordem { get; set; }
        public bool Ativo { get; set; }
        public Guid? IdPai { get; set; }
        public Guid? IdSecao { get; set; }
        public string Linque { get; set; }
        public string Alt { get; set; }
    }
}
