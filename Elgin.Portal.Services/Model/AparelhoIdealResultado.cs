using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class AparelhoIdealResultado
    {
        public Guid Id { get; set; }
        public string NomeProduto { get; set; }
        public string NomeModelo { get; set; }
        public string CodigoLegado { get; set; }
        public string CodigoProduto { get; set; }
        public string CodigoLinha { get; set; }
        public string CodigoSublinha { get; set; }
        public string ImagemUrl
        {
            get
            {
                if (Arquivos != null && Arquivos.Count > 0)
                {
                    return Arquivos[0].Caminho;
                }

                return "";
            }
        }

        public virtual List<Arquivo> Arquivos { get; set; }
    }
}
