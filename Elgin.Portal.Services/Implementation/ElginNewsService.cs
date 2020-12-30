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
    public class ElginNewsService : AbstractService<ElginNewsService>
    {
        public ElginNewsService(string connectionString) : base(connectionString) { }

        public List<ElginNews> Listar()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<ElginNews>(
                @"
                    SELECT Id
                      ,Nome
                      ,Ano
                      ,Numero
                      ,Imagem
                      ,Arquivo
                      ,Data
                      ,Ativo
                  FROM 
                    ElginNews
                  WHERE Ativo = 1
                  ORDER BY 
                    ano desc, numero
                ").ToList();
            }
        }

        public bool Salvar(ElginNews model)
        {
            if(model.Id == 0)
            {
                return Incluir(model);
            }
            else
            {
                return Atualizar(model);
            }
        }

        public bool Remover(int id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var execute = conexao.Execute(
                @"
                  DELETE FROM ElginNews wHERE Id = @id
                ", new { id });

                return execute > 0;
            }
        }

        public bool Incluir(ElginNews model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var execute = conexao.Execute(
                @"
                    INSERT INTO [ElginNews]
                                ([Nome]
                                ,[Ano]
                                ,[Numero]
                                ,[Imagem]
                                ,[Arquivo]
                                ,[Ativo])
                            VALUES
                                (@Nome
                                ,@Ano
                                ,@Numero
                                ,@Imagem
                                ,@Arquivo
                                ,@Ativo)
                ", model);

                return execute > 0;
            }
        }

        public bool Atualizar(ElginNews model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var execute = conexao.Execute(
                @"
                    UPDATE [ElginNews]
                       SET [Nome] = @Nome
                          ,[Ano] = @Ano
                          ,[Numero] = @Numero
                          ,[Imagem] = @Imagem
                          ,[Arquivo] = @Arquivo
                          ,[Ativo] = @Ativo
                     WHERE id = @id
                ", model);

               return execute > 0;
            }
        }
    }
}
