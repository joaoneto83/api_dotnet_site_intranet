using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.Transactions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Elgin.Portal.Services.Implementation
{
    public class DependenteService : AbstractService<DependenteService>
    {

        public DependenteService(string connectionString) : base(connectionString) { }

        public List<Dependente> ListarPorIdUsuario(Guid idUsuario)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Dependente>(
                @"
                    SELECT [Id]
                          ,[NomeDependente]
                          ,[Celular]
                          ,(SELECT CONVERT(varchar, [DataNascimento], 23)) AS DataNascimento
                          ,[IdUsuario]
                          ,[IdTipoDependente]
                      FROM [Dependente]
                      WHERE IdUsuario = @idUsuario
                ", new { idUsuario }).ToList();
            }

        }

        public void SalvarDependentes(Usuario model)
        {
            RemoverDependentes(model.Id);

            foreach (var dependente in model.Dependentes)
            {
                if (dependente.Id == Guid.Empty)
                    dependente.Id = Guid.NewGuid();

                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Execute(
                          @"INSERT INTO [Dependente]
                                        ([Id]
                                        ,[NomeDependente]
                                        ,[Celular]
                                        ,[DataNascimento]
                                        ,[IdUsuario]
                                        ,[IdTipoDependente])
                                VALUES
                                        (@Id
                                        ,@NomeDependente
                                        ,@Celular
                                        ,@DataNascimento
                                        ,@IdUsuario
                                        ,@IdTipoDependente)"
                    , new
                    {
                        dependente.Id,
                        dependente.NomeDependente,
                        dependente.Celular,
                        dependente.DataNascimento,
                        IdUsuario = model.Id,
                        dependente.IdTipoDependente
                    });
                }


            }
        }

        private void RemoverDependentes(Guid idUsuario)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [Dependente]
                         WHERE IdUsuario = @idUsuario
                ", new
                {
                    idUsuario
                });
            }
        }
    }
}
