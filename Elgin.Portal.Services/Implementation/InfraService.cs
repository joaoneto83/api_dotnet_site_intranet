using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.Transactions;
using System.IO;

namespace Elgin.Portal.Services.Implementation
{
    public class InfraService : AbstractService<InfraService>
    {
        public InfraService(string connectionString) : base(connectionString)
        {}
        

        public void GravaLog(Log model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                $@"
                    INSERT INTO [dbo].[Log]
                               ([Message]
                               ,[Url]
                               ,[Stack])
                         VALUES
                               (
                               @Message
                               ,@Url
                               ,@Stack)
                "
                , model);
            }
            
        }

        public void GravaLogApi(LogApi model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                $@"
                    INSERT INTO [dbo].[LogApi]
                               ([EventId]
                               ,[LogLevel]
                               ,[Message]
                               ,[CreatedTime])
                         VALUES
                               (@EventId
                               ,@LogLevel
                               ,@Message
                               ,@CreatedTime)
                "
                , model);
            }
        }
    }
}
