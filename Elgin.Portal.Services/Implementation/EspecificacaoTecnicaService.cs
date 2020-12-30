using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Elgin.Portal.Services.Implementation.Shared;
using Elgin.Portal.Services.Model;

namespace Elgin.Portal.Services.Implementation
{
    public class EspecificacaoTecnicaService : AbstractService<EspecificacaoTecnicaService>
    {
        public EspecificacaoTecnicaService(string connectionString) : base(connectionString)
        {
        }

        public List<EspecificacaoTecnica> ListarEspecificacoesTecnicas(Guid idSublinha, Guid idProduto, bool? ativo, int? idioma)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<EspecificacaoTecnica>(
                    @"
                    SELECT 
                        ET.[Id],
                        ET.[CodigoEspecificacao],
                        CASE WHEN ETI.NomeEspecificacao IS NULL OR ETI.NomeEspecificacao = '' OR @idioma IS NULL THEN
		                    ET.NomeEspecificacao
	                    ELSE
		                    ETI.NomeEspecificacao
	                    END as NomeEspecificacao,
                        CASE WHEN PETI.Valor IS NULL OR PETI.Valor = '' OR @idioma IS NULL THEN
		                    ISNULL(PET.Valor,'-')
	                    ELSE
		                    ISNULL(PETI.Valor,'-')
                        END as Valor
                    FROM [dbo].[EspecificacaoTecnica] ET
                    LEFT JOIN [dbo].[ProdutoEspecificacaoTecnica] PET
                        ON PET.IdEspecificacaoTecnica = ET.Id AND PET.IdProduto = @idProduto
                    LEFT JOIN EspecificacaoTecnicaIdioma as ETI
                        ON ET.Id = ETI.IdEspecificacaoTecnica AND ETI.CodigoIdioma = @idioma
                    LEFT JOIN ProdutoEspecificacaoTecnicaIdioma AS PETI
                        ON PETI.IdProdutoEspecificacaoTecnica = CONCAT(PET.IdProduto, ET.Id) AND PETI.CodigoIdioma = @idioma
                    WHERE ET.IdSubLinha = @idSublinha AND (@ativo IS NULL OR ET.Ativo = @ativo)
                ", new { idSublinha, idProduto, ativo, idioma }).ToList();
            }
        }

        public List<EspecificacaoTecnica> ListarEspecificacoesTecnicas(string codigoSublinha, int idioma)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<EspecificacaoTecnica>(
                    @"
                    SELECT 
                        ET.[Id],
                        ET.[CodigoEspecificacao],
                        CASE WHEN ETI.NomeEspecificacao IS NULL OR ETI.NomeEspecificacao = '' THEN
		                    ET.NomeEspecificacao
	                    ELSE
		                    ETI.NomeEspecificacao
	                    END as NomeEspecificacao
                    FROM [dbo].[EspecificacaoTecnica] ET
                    JOIN SubLinha 
                        ON SubLinha.Id = ET.IdSubLinha
                    LEFT JOIN EspecificacaoTecnicaIdioma as ETI
                        ON ET.Id = ETI.IdEspecificacaoTecnica AND ETI.CodigoIdioma = @idioma
                    WHERE (SubLinha.CodigoSubLinha = 'COMPRESSORES' AND SubLinha.Ativo = 1)
                        AND ET.Comparativo = 1 AND ET.Ativo = 1
                ", new { codigoSublinha, idioma }).ToList();
            }
        }
    }
}
