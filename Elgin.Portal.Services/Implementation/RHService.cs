using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;

namespace Elgin.Portal.Services.Implementation
{
    public class RHService : AbstractService<RHService>
    {
        private ImagemService imagemService;

        public RHService(string connectionString) : base(connectionString) {

            imagemService = new ImagemService(connectionString);

        }

        public RH GetRH()
        {
            var model = new RH();

            model.Banners = imagemService.Banners("Intranet-RH", "1", null);

            return model;
        }
    }
}
