using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class FileResponse
    {
        public bool sucesso { get; set; }
        public string erro { get; set; }
        public string caminho { get; set; }
        public string arquivo { get; set; }
    }
}
