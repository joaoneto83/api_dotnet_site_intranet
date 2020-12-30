using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;

namespace Elgin.Portal.Services.Implementation
{
    public class UserService : AbstractService<UserService>
    {
        public UserService(string connectionString) : base(connectionString) { }

        
    }
}
