using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Traducao
    {
        public object id          { get; set; }
        public Guid? idEN        { get; set; }
        public Guid? idES        { get; set; }
        public String pt        { get; set; }
        public String en        { get; set; }
        public String es        { get; set; }
        public String tabela    { get; set; }
        public String campo     { get; set; }
        public String campoRef { get; set; }
    }
}
