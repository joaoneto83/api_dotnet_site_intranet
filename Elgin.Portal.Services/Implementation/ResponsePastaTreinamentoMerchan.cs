using Elgin.Portal.Services.Model;
using System.Collections.Generic;

namespace Elgin.Portal.Services.Implementation
{
    public class ResponsePastaTreinamentoMerchan
    {
        public object IdPai { get; internal set; }
        public List<PastaTreinamentoMerchan> Pastas { get; internal set; }
        public List<TreinamentoMerchan> Treinamentos { get; internal set; }
        public object Id { get; internal set; }
    }
}