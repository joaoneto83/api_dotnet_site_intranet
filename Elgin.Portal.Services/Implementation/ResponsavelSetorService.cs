using Dapper;
using Elgin.Portal.Services.Implementation.Shared;
using Elgin.Portal.Services.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Elgin.Portal.Services.Implementation
{
    public class ResponsavelSetorService : AbstractService<ResponsavelSetorService>
    {
        public ResponsavelSetorService(string connectionString) : base(connectionString) { }

        public List<ResponsavelSetor> Listar()
        {
            var retorno = new List<ResponsavelSetor>();

            var sql = @"
                    SELECT 
                        Id,
                        Setor,
                        Nome,
                        Telefone,
                        Ativo
                    FROM ResponsavelSetor
                    WHERE Ativo = 1
                    ORDER BY Setor
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<ResponsavelSetor>(sql).ToList();
            }

            return retorno;
        }

        public bool Salvar(ResponsavelSetor model)
        {
            try
            {
                if (model.Id != Guid.Empty)
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             UPDATE [ResponsavelSetor]
                                SET [Setor] = @Setor
                                    ,[Nome] = @Nome
                                    ,[Telefone] = @Telefone
                                    ,[Ativo] = @Ativo
                                WHERE Id = @Id
                        ", model);
                    }
                }

                else
                {
                    model.Id = Guid.NewGuid();

                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             INSERT INTO [ResponsavelSetor]
                                   ([Id]
                                   ,[Setor]
                                   ,[Nome]
                                   ,[Telefone]
                                   ,[Ativo])
                             VALUES
                                   (@Id
                                   ,@Setor
                                   ,@Nome
                                   ,@Telefone
                                   ,@Ativo)
                        ", model);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}

