using System;

namespace Elgin.Portal.Services.Model
{
    public class Aviso
    {
        public Guid Id { get; set; }
        public Guid IdTipoAviso { get; set; }
        public string CodigoTipoAviso { get; set; }
        public string Modulo { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string DataAviso { get; set; }
        public bool Ativo { get; set; }
    }
}
