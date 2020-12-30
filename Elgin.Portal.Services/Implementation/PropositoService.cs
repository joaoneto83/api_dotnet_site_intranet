using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.IO;
using System.Transactions;

namespace Elgin.Portal.Services.Implementation
{
    public class PropositoService : AbstractService<PropositoService>
    {

        public PropositoService(string connectionString) : base(connectionString) { }

        public Proposito BuscarProposito()
        {
            var retorno = new Proposito();

            var sql = @"
                  SELECT [Id]
                        ,[Texto]
                    FROM [Proposito]
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Proposito>(sql).FirstOrDefault();
            }

            return retorno;
        }

        public Proposito SalvarProposito(Proposito model)
        {
            using (var transaction = new TransactionScope())
            {
                if (model.Id == Guid.Empty)
                {
                    model.Id = Guid.NewGuid();
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(@"INSERT INTO [Proposito]
                                                          ([Id]
                                                          ,[Texto])
                                                   VALUES
                                                          (@Id
                                                          ,@Texto)", model);
                    }
                }
                else
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(@"UPDATE [Proposito]
                                                 SET [Texto] = @Texto
                                              WHERE Id = @Id", model);
                    }
                }

                transaction.Complete();
            }

            return model;
        }
    }
}
