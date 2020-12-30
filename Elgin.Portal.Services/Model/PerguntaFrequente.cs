using System;
using System.Collections.Generic;
using Elgin.Portal.Services.Implementation;

namespace Elgin.Portal.Services.Model
{
    public class PerguntaFrequente
    {
        public Guid Id { get; set; }
        public string Pergunta { get; set; }
        public string Resposta { get; set; }
        public string CodigoComponente { get; set; }
    }
}
