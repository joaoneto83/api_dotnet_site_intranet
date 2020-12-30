using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Log
    {
        public Guid Id { get; set; }
        public DateTime Data { get; set; }
        public String Url { get; set; }
        public String Message { get; set; }
        public String Stack { get; set; }
    }
}
