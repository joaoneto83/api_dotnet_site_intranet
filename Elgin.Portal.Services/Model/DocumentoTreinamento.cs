using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class DocumentoTreinamento
    {
        public Guid IdLinha { get; set; }
        public Guid? IdSubLinha { get; set; }
        public Guid? IdProduto { get; set; }
        public List<Arquivo> Arquivos { get; set; }
    }
}
