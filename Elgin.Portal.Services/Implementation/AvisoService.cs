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
    public class AvisoService : AbstractService<AvisoService>
    {

        public AvisoService(string connectionString) : base(connectionString) { }

        public List<Aviso> ListarPorModulo(string modulo)
        {
            var retorno = new List<Aviso>();

            var sql = @"
                  SELECT Aviso.Id
                        ,IdTipoAviso
                        ,Modulo
                        ,Titulo
                        ,Descricao
                        ,(SELECT CONVERT(varchar, DataAviso, 23)) AS DataAviso
                        ,Aviso.Ativo
	                    ,TipoAviso.CodigoTipoAviso
                    FROM dbo.Aviso
                    INNER JOIN TipoAviso ON
                    TipoAviso.Id = Aviso.IdTipoAviso
                    WHERE Aviso.Modulo = @modulo AND Aviso.Ativo = 1
                    ORDER BY Aviso.DataAviso DESC
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Aviso>(sql, new { modulo }).Distinct().ToList();
            }

            return retorno;
        }

        public Aviso ProcurarAvisoPorId(Guid id)
        {
            var retorno = new Aviso();

            var sql = @"
                  SELECT Aviso.Id
                        ,IdTipoAviso
                        ,Modulo
                        ,Titulo
                        ,Descricao
                        ,DataAviso
                        ,Aviso.Ativo
	                    ,TipoAviso.CodigoTipoAviso
                    FROM dbo.Aviso
                    INNER JOIN TipoAviso ON
                    TipoAviso.Id = Aviso.IdTipoAviso
                    WHERE Aviso.Id = @id
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Aviso>(sql, new { id }).FirstOrDefault();
            }

            return retorno;
        }

        public bool Salvar(Aviso model)
        {
            try
            {
                if (model.Id != Guid.Empty)
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             UPDATE [Aviso]
                                SET [Titulo] = @Titulo
                                    ,[Descricao] = @Descricao
                                    ,[DataAviso] = @DataAviso
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
                             INSERT INTO [Aviso]
                                   ([Id]
                                   ,[IdTipoAviso]
                                   ,[Modulo]
                                   ,[Titulo]
                                   ,[Descricao]
                                   ,[DataAviso]
                                   ,[Ativo])
                             VALUES
                                   (@Id
                                   ,(Select Id FROM TipoAviso where CodigoTipoAviso = @CodigoTipoAviso)
                                   ,@Modulo
                                   ,@Titulo
                                   ,@Descricao
                                   ,@DataAviso
                                   ,@Ativo)
                        ", model);
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
