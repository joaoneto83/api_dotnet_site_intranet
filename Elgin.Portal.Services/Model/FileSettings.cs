using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class FileSettings
    {
        public enum FyleType { Blob = 1, Servidor = 2}
        
        public FyleType Type { get; set; }
        public string ServerPath { get; set; }
        public string WebPath { get; set; }
        public string BlobConnection { get; set; }
    }
}
