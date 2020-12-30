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
    public class SetorService : AbstractService<SetorService>
    {

        public SetorService(string connectionString) : base(connectionString) { }

        public List<Setor> Listar()
        {
            var retorno = new List<Setor>();

            var sql = @"
                  SELECT [Id]
                        ,[NomeSetor]
                        ,[CodigoSetor]
                        ,[Ativo]
                    FROM [dbo].[Setor]
                    WHERE Ativo = 1
                    ORDER BY NomeSetor
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Setor>(sql).Distinct().ToList();
            }

            return retorno;
        }

        public Setor RetornarSetorPorId(Guid idSetor)
        {
            var retorno = new Setor();

            var sql = @"
                  SELECT [Id]
                        ,[NomeSetor]
                        ,[CodigoSetor]
                        ,[Ativo]
                    FROM [dbo].[Setor]
                    WHERE Id = @idSetor
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Setor>(sql, new { idSetor }).FirstOrDefault();
            }

            return retorno;
        }
    }
}
