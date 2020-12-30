using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class LogApi
    {
        public Guid Id { get; set; }
        public int EventId { get; set; }
        public String LogLevel { get; set; }
        public String Message { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
