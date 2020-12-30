using Elgin.Portal.Services.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elgin.Portal.Api.Controllers.Shared
{
    public class Config
    {
        IConfiguration _configuration;

        public Config(IConfiguration configuration) {
            _configuration = configuration;
        }

        public string ConnectionString
        {
            get
            {
                return _configuration.GetValue<string>("DBInfo:ConnectionString");
            }
        }

        public EmailSettings EmailSettings
        {
            get
            {
                var myConfig = new EmailSettings();
                _configuration.GetSection("EmailSettings").Bind(myConfig);

                return myConfig;
            }
        }
        public FileSettings FileSettings
        {
            get
            {
                var myConfig = new FileSettings();
                _configuration.GetSection("FileSettings").Bind(myConfig);

                return myConfig;
            }
        }

    }
}
