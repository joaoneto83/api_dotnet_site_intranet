using Dapper;
using Elgin.Portal.Services.Implementation.Shared;
using Elgin.Portal.Services.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Elgin.Portal.Services.Implementation
{
    public class EventoRefrigeracaoService : AbstractService<EventoRefrigeracaoService>
    {
        public EventoRefrigeracaoService(string connectionString) : base(connectionString) { }

        public List<EventoRefrigeracao> Listar()
        {
            var retorno = new List<EventoRefrigeracao>();

            var sql = @"
                  SELECT EventoRefrigeracao.Id
                        ,Titulo
                        ,DataDe
                        ,DataAte
                        ,Local
                        ,CaminhoImagem
                        ,IdTipoEventoRefrigeracao
                        ,Link
                        ,TipoEventoRefrigeracao.Nome AS NomeTipoEventoRefrigeracao
                    FROM EventoRefrigeracao
                    LEFT JOIN TipoEventoRefrigeracao ON TipoEventoRefrigeracao.Id = EventoRefrigeracao.IdTipoEventoRefrigeracao
                    ORDER BY EventoRefrigeracao.DataDe
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<EventoRefrigeracao>(sql).Distinct().ToList();
            }

            return retorno;
        }

        public EventoRefrigeracao ProcuraEventoRefrigeracaoPorId(Guid idEventoRefrigeracao)
        {
            var retorno = new EventoRefrigeracao();

            var sql = @"
                  SELECT EventoRefrigeracao.Id
                        ,Titulo
                        ,DataDe
                        ,DataAte
                        ,Local
                        ,CaminhoImagem
                        ,IdTipoEventoRefrigeracao
                        ,Link
                        ,TipoEventoRefrigeracao.Nome AS NomeTipoEventoRefrigeracao
                    FROM EventoRefrigeracao
                    LEFT JOIN TipoEventoRefrigeracao ON TipoEventoRefrigeracao.Id = EventoRefrigeracao.IdTipoEventoRefrigeracao
                    WHERE EventoRefrigeracao.Id = @idEventoRefrigeracao
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<EventoRefrigeracao>(sql, new { idEventoRefrigeracao }).FirstOrDefault();
            }

            return retorno;
        }

        public bool Salvar(EventoRefrigeracao model)
        {
            try
            {
                if (model.Id != Guid.Empty)
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             UPDATE [EventoRefrigeracao]
                                SET [Titulo] = @Titulo
                                   ,[DataDe] = @DataDe
                                   ,[DataAte] = @DataAte
                                   ,[Local] = @Local
                                   ,[CaminhoImagem] = @CaminhoImagem
                                   ,[IdTipoEventoRefrigeracao] = @IdTipoEventoRefrigeracao
                                   ,[Link] = @Link
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
                             INSERT INTO [EventoRefrigeracao]
                                        ([Id]
                                        ,[Titulo]
                                        ,[DataDe]
                                        ,[DataAte]
                                        ,[Local]
                                        ,[CaminhoImagem]
                                        ,[IdTipoEventoRefrigeracao]
                                        ,[Link])
                                    VALUES
                                        (@Id
                                        ,@Titulo
                                        ,@DataDe
                                        ,@DataAte
                                        ,@Local
                                        ,@CaminhoImagem
                                        ,@IdTipoEventoRefrigeracao
                                        ,@Link)
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

        public bool Remover(Guid idEvento)
        {
            try
            { 
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Execute(
                    @"
                           DELETE FROM EventoRefrigeracao WHERE Id = @idEvento

                    ", new { idEvento });
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public List<EventoRefrigeracao> Listar(int? idTipoEventoRefrigeracao, int? mes, string cultura, bool apenasTreinamentos)
        {
            var retorno = new List<EventoRefrigeracao>();
            var ano = DateTime.Now.Year;

            if (mes < DateTime.Now.Month)
                ano++;

            var idioma = cultura == "pt-Br" ? 1 : cultura == "en-US" ? 2 : 3;
            
            var sql = @"
                  SELECT EventoRefrigeracao.Id
                        ,CASE WHEN ERI.Titulo IS NULL OR ERI.Titulo = '' THEN
		                    EventoRefrigeracao.Titulo
	                     ELSE
		                    ERI.Titulo
                         END AS Titulo
                        ,DataDe
                        ,DataAte
                        ,Local
                        ,CaminhoImagem
                        ,IdTipoEventoRefrigeracao
                        ,Link
                        ,TipoEventoRefrigeracao.Nome AS NomeTipoEventoRefrigeracao
                        ,@cultura AS Cultura
                    FROM EventoRefrigeracao
                    LEFT JOIN TipoEventoRefrigeracao ON TipoEventoRefrigeracao.Id = EventoRefrigeracao.IdTipoEventoRefrigeracao
                    LEFT JOIN EventoRefrigeracaoIdioma ERI
                    ON EventoRefrigeracao.Id = ERI.IdEventoRefrigeracao AND ERI.CodigoIdioma = @idioma
                    WHERE 
                        (DATEDIFF(day, GETDATE(), DataDe) >= 0 OR DATEDIFF(day, GETDATE(), DataAte) >= 0) AND
	                    (
			                (
			                    (@idTipoEventoRefrigeracao IS NULL AND 
			                    (
				                    @apenasTreinamentos = 0 OR 
				                    (@apenasTreinamentos = 1 AND (IdTipoEventoRefrigeracao = 1 OR IdTipoEventoRefrigeracao = 2))
			                    )) OR 
			                    IdTipoEventoRefrigeracao = @idTipoEventoRefrigeracao
		                    ) AND 
			                (
                                (DataAte IS NULL AND YEAR(DataDe) = @ano AND MONTH(DataDe) = @mes) OR
				                (YEAR(DataDe) = YEAR(DataAte) AND YEAR(DataDe) = @ano AND MONTH(DataDe) <= @mes AND MONTH(DataAte) >= @mes) OR 
				                (
					                YEAR(DataDe) != YEAR(DataAte) AND 
					                (
						                (YEAR(DataDe) = @ano AND MONTH(DataDe) <= @mes) OR 
						                (YEAR(DataAte) = @ano AND MONTH(DataAte) >= @mes)
					                )
				                )
				                OR @mes IS NULL
			                )
		                )
                    ORDER BY EventoRefrigeracao.DataDe
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<EventoRefrigeracao>(sql, new { idTipoEventoRefrigeracao, ano, mes, cultura, apenasTreinamentos, idioma }).Distinct().ToList();
            }

            return retorno;
        }
    }
}
