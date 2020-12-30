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
    public class AssistenciaVinculadaService : AbstractService<AssistenciaVinculadaService>
    {

        public AssistenciaVinculadaService(string connectionString) : base(connectionString) { }

        public List<AssistenciaVinculada> Listar()
        {
            var retorno = new List<AssistenciaVinculada>();

            var sql = @"
                  SELECT AV.[Id]
                        ,[Documento]
                        ,AV.[Ativo]
	                    ,SubLinha.NomeSubLinha
	                    ,SubLinha.Id
	                    ,SubLinha.CodigoSubLinha
                FROM [AssistenciaVinculada] AS AV
                LEFT JOIN AssistenciaVinculadaSubLinha AVS ON
                AVS.IdAssistenciaVinculada = AV.Id
                LEFT JOIN SubLinha ON
                SubLinha.Id = AVS.IdSubLinha
                WHERE AV.Ativo = 1
            ";
            
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, AssistenciaVinculada>();

                retorno = conexao.Query<AssistenciaVinculada, SubLinha, AssistenciaVinculada>(sql,
                (assistencia, sublinha) =>
                {
                    AssistenciaVinculada assistenciaEntry;

                    if (!dictionary.TryGetValue(assistencia.Id, out assistenciaEntry))
                    {
                        assistenciaEntry = assistencia;
                        assistenciaEntry.SubLinhas = new List<SubLinha>();
                        dictionary.Add(assistenciaEntry.Id, assistenciaEntry);
                    }

                    if (sublinha != null)
                        assistenciaEntry.SubLinhas.Add(sublinha);

                    return assistenciaEntry;

                }, splitOn: "NomeSubLinha").Distinct().ToList();
            }

            return retorno;
        }

        public AssistenciaVinculada GetAssistenciaPorId(Guid idAssistencia)
        {
            var retorno = new AssistenciaVinculada();

            var sql = @"
                  SELECT [Id]
                        ,[Documento]
                        ,[Ativo]
                FROM [AssistenciaVinculada]
                WHERE Ativo = 1 AND Id = @idAssistencia
            ";

            var sqlSublinha = @"
                SELECT [IdSublinha] AS Id
                FROM [AssistenciaVinculadaSublinha]
                WHERE IdAssistenciaVinculada = @idAssistencia
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.QueryFirstOrDefault<AssistenciaVinculada>(sql, new { idAssistencia });
                retorno.SubLinhas = conexao.Query<SubLinha>(sqlSublinha, new { idAssistencia }).ToList();
            }

            return retorno;
        }

        public List<AssistenciaVinculada> BuscarAssistencias(Guid idSubLinha)
        {
            var retorno = new List<AssistenciaVinculada>();

            var sql = @"
                  SELECT DISTINCT AV.[Id]
                        ,[Documento]
                        ,AV.[Ativo]
	                    ,SubLinha.NomeSubLinha
	                    ,SubLinha.Id
	                    ,SubLinha.CodigoSubLinha
                FROM [AssistenciaVinculada] AS AV
                LEFT JOIN AssistenciaVinculadaSubLinha AVS ON
                AVS.IdAssistenciaVinculada = AV.Id
                LEFT JOIN SubLinha ON
                SubLinha.Id = AVS.IdSubLinha
                WHERE AV.Ativo = 1 
                AND AVS.IdSubLinha = @idSubLinha
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<AssistenciaVinculada>(sql, new { idSubLinha }).Distinct().ToList();
            }

            return retorno;
        }

        public bool VincularSubLinhas(Guid idAssistencia, string documento, string idsSubLinhas)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    if (idAssistencia == Guid.Empty)
                    {
                        idAssistencia = Guid.NewGuid();
                        IncluirAssistencia(idAssistencia, documento);
                    }
                    else
                    {
                        RemoverSubLinhasVinculadas(idAssistencia);
                    }

                    var listaIdsSubLinhas = Array.ConvertAll(idsSubLinhas.Split(','), s => new Guid(s)).ToList();
                    IncluirSubLinhasVinculadas(idAssistencia, listaIdsSubLinhas);

                    transaction.Complete();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void IncluirAssistencia(Guid idAssistencia, string documento)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    INSERT INTO [dbo].[AssistenciaVinculada]
                        ([Id]
                        ,[Documento]
                        ,[Ativo])
                    VALUES
                        (@idAssistencia
                        ,@documento
                        ,1)
               ", new { idAssistencia, documento });
            }
        }

        private void RemoverSubLinhasVinculadas(Guid idAssistencia)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    DELETE
                        FROM [AssistenciaVinculadaSubLinha]
                    WHERE
                        IdAssistenciaVinculada = @idAssistencia 
               ", new { idAssistencia });
            }
        }

        private void IncluirSubLinhasVinculadas(Guid idAssistencia, List<Guid> idsSubLinhas)
        {

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                string sql = string.Empty;

                foreach (var idSubLinha in idsSubLinhas)
                    sql += $@"INSERT INTO [dbo].[AssistenciaVinculadaSubLinha]
                                  ([IdAssistenciaVinculada]
                                  ,[IdSubLinha])
                              VALUES
                                  ('{idAssistencia}'
                                  ,'{idSubLinha}')";

                conexao.Execute(sql);
            }
        }
    }
}
