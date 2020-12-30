using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Model
{
    public class Email
    {
        public Email()
        {
            Para = new List<string>();
            CCo = new List<string>();
        }

        public string Assunto { get; set; }
        public string Corpo { get; set; }
        public List<string> Para { get; set; }
        public List<string> CCo { get; internal set; }
    }
}
