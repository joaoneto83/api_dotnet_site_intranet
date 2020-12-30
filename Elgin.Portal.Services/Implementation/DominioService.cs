using Elgin.Portal.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Elgin.Portal.Services.Model;
using System.Linq;

namespace Elgin.Portal.Services.Implementation
{
    public class DominioService : AbstractService<DominioService>
    {
        public DominioService(string connectionString) : base(connectionString) { }

        public Estado EstadoPorUF(string uf)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Estado>(
                @"
                    SELECT 
                        
                        [Sigla],
                        [Descricao],
                        [TarifaSolar]
                    FROM [dbo].[Estado]
                    WHERE Sigla = @uf
                ", new { uf }).FirstOrDefault();
            }
        }

        public List<Estado> ListarEstados(int idPais = 1)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Estado>(
                @" 
                    SELECT 
                        
                        [Sigla],
                        [Descricao],
                        [TarifaSolar]
                    FROM [dbo].[Estado]
                    WHERE IdPais = @idPais

                ", new { idPais }).ToList();
            }
        }

        public Cidade CidadePorId(int id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Cidade>(
                    @"
                    SELECT 
                        C.[Id], 
                        C.[Descricao],
                        C.Latitude,
                        C.Longitude,
                        C.SolarMediaAnual
                    FROM [dbo].[Cidade] C
                    WHERE Id = @id

                ", new { id }).FirstOrDefault();
            }
        }

        public List<Cidade> ListaCidades(string estado)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Cidade>(
                    @"
                    SELECT C.[Id], C.[Descricao]
                    FROM [dbo].[Cidade] C
                    INNER JOIN [dbo].[Estado] E ON E.Id = C.IdEstado AND E.Sigla = @estado
                    ORDER BY C.[Descricao]

                ", new { estado }).ToList();
            }
        }

        public List<Pais> ListaPaises()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Pais>(
                @"
                    SELECT 
                        [Id],
                        [Nome],
                        [Sigla]
                    FROM [dbo].[Pais]

                ", new { }).ToList();
            }
        }

        public List<Segmento> ListaSegmentos()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Segmento>(
                @"
                    SELECT 
                        [Id],
                        [Nome],
                        [Codigo]
                    FROM [dbo].[Segmento]
                    WHERE [Ativo] = 1
                    ORDER BY [Ordem]

                ", new { }).ToList();
            }
        }

        public List<TipoEventoRefrigeracao> ListarTiposEventoRefrigeracao()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<TipoEventoRefrigeracao>(
                @"
                    SELECT 
                        [Id],
                        [Nome],
                        [Codigo]
                    FROM [dbo].[TipoEventoRefrigeracao]
                    WHERE [Ativo] = 1
                    ORDER BY [Ordem]

                ", new { }).ToList();
            }
        }

        public List<SolarIntegrador> getSolarIntegradores(string estado, int? idCidade)
        {
            if (idCidade == 0)
                idCidade = null;

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<SolarIntegrador>(@"
                    SELECT [SolarIntegrador].[Id]
                          ,[SolarIntegrador].[UF]
                          ,[SolarIntegrador].[Ativo]
                          ,[SolarIntegrador].[CNPJ]
                          ,[SolarIntegrador].[Nome]
                          ,[SolarIntegrador].[Endereco]
                          ,[SolarIntegrador].[Telefone]
                          ,[SolarIntegrador].[Email]
                          ,[SolarIntegrador].[Site]
                          ,[SolarIntegrador].[IdCidade]
	                      ,Estado.Descricao as NomeEstado
	                      ,Estado.Sigla as UF
                    FROM 
	                    [SolarIntegrador]
                    JOIN 
	                    Estado
                    ON 
	                    Estado.Sigla = SolarIntegrador.UF      
                    WHERE 
                        Estado.Sigla = @estado AND
                        (SolarIntegrador.IdCidade = @idCidade OR @idCidade IS NULL)
                    ORDER BY [SolarIntegrador].[Nome]
                    ", new { estado, idCidade }).ToList();
            }
        }
    }
}
