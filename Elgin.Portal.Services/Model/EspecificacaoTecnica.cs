using System;

namespace Elgin.Portal.Services.Model
{
    public class EspecificacaoTecnica
    {
        public Guid Id { get; set; }
        public Guid? IdProduto { get; set; }
        public string CodigoEspecificacao { get; set; }
        public string NomeEspecificacao { get; set; }
        public string Valor { get; set; }
        public bool Comparativo { get; set; }
        public bool Ativo { get; set; }
    }
}
