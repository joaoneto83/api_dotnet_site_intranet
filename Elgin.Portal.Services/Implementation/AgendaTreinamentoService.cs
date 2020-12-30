using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.IO;

namespace Elgin.Portal.Services.Implementation
{
    public class AgendaTreinamentoService : AbstractService<AgendaTreinamentoService>
    {

        public AgendaTreinamentoService(string connectionString) : base(connectionString) { }

        public List<AgendaTreinamento> Listar()
        {
            var retorno = new List<AgendaTreinamento>();

            var sql = @"
                  SELECT [Id]
                        ,(SELECT CONVERT(varchar, DataDe, 103)) AS DataDe
                        ,(SELECT CONVERT(varchar, DataAte, 103)) AS DataAte
                        ,[Empresa]
                        ,[Cidade]
                        ,[Estado]
                    FROM [AgendaTreinamentos]
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<AgendaTreinamento>(sql).Distinct().ToList();
            }

            return retorno;
        }
    }
}
