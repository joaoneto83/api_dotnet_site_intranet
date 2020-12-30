using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.Transactions;
using Microsoft.AspNetCore.Http;

namespace Elgin.Portal.Services.Implementation
{
    public class ImagemService : AbstractService<ImagemService>
    {
        ArquivoService arquivoService;

        public ImagemService(string connectionString) : base(connectionString)
        {
        }

        public ImagemService(string connectionString, FileSettings fileSettings) : base(connectionString, fileSettings) {
            arquivoService = new ArquivoService(connectionString, fileSettings);
        }

        public List<Banner> Banners(string modulo, string componente, int? idioma)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Banner>(
                @"
                    SELECT Banner.id
                      ,Banner.modulo 
                      ,Banner.componente 
                      ,Banner.posicao 
                      ,Banner.caminho 
                      ,CASE WHEN BannerIdioma.texto1 IS NULL OR BannerIdioma.texto1 = '' OR @idioma IS NULL THEN
		                    Banner.texto1
	                   ELSE
		                    BannerIdioma.texto1
	                   END as texto1
                      ,CASE WHEN BannerIdioma.texto2 IS NULL OR BannerIdioma.texto2 = '' OR @idioma IS NULL THEN
		                    Banner.texto2
	                   ELSE
		                    BannerIdioma.texto2
                       END AS texto2
                      ,CASE WHEN BannerIdioma.texto3 IS NULL OR BannerIdioma.texto3 = '' OR @idioma IS NULL THEN
		                    Banner.texto3
	                   ELSE
		                    BannerIdioma.texto3
                       END AS texto3
                      ,Banner.link
                      ,Banner.cor
                      ,Banner.ativo
                      ,Arquivo.caminho
                  FROM Banner
                  JOIN Arquivo
                  ON Arquivo.IdPai = Banner.Id
                  LEFT JOIN BannerIdioma
                    ON Banner.Id = BannerIdioma.IdBanner AND BannerIdioma.CodigoIdioma = @idioma
                  WHERE 
                      Modulo = @modulo AND Arquivo.Ativo = 1 AND 
                     (componente = '' OR componente = @componente)
                ORDER BY 
                    posicao
                ", new { modulo, componente, idioma }).ToList();
            }
        }

        public List<Arquivo> ImagensPorCodigo(string codigo)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Arquivo>(
                @"
                    SELECT Arquivo.[Id]
                          ,Arquivo.[IdTipoArquivo]
                          ,Arquivo.[NomeArquivo]
                          ,Arquivo.[Caminho]
                          ,Arquivo.[Ordem]
                          ,Arquivo.[Ativo]
                          ,Arquivo.[IdPai]
                          ,Arquivo.[IdSecao]
                      FROM [Arquivo]
                      INNER JOIN
                        TipoArquivo
                      ON
                        TipoArquivo.Id = IdTipoArquivo
                      WHERE 
                        TipoArquivo.CodigoTipoArquivo = @codigo AND
                        TipoArquivo.Ativo = 1 AND 
                        Arquivo.Ativo = 1
                      ORDER BY NomeArquivo
                ", new { codigo }).ToList();
            };
        }

        public bool RemoverBanner(Guid idBanner)
        {
            try
            {
                var execute = 0;

                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    execute = conexao.Execute(
                    @"

                    DELETE FROM ARQUIVO WHERE IdPai = @idBanner
                    DELETE FROM BANNER WHERE Id = @idBanner

                ", new { idBanner });
                }

                return execute > 0;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool SalvarBanner(Banner banner, IFormFile file)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    var idPai = Guid.Empty;

                    if (banner.id == Guid.Empty)
                    {
                        idPai = IncluiBanner(banner);
                    }
                    else
                    {
                        idPai = EditarBanner(banner);
                    }

                    if(file != null)
                    {
                        if (!arquivoService.DelArquivoPorIdPaiTipo(idPai, "imgBanner")) return false;

                        var arq = arquivoService.SalvarArquivo(file, "imgBanner", "", idPai.ToString()).Result;

                        if (!arq.sucesso) return false;
                        
                        var modelArq = new Arquivo
                        {
                            IdPai = idPai,
                            CodigoTipoArquivo = "imgBanner",
                            Caminho = arq.caminho,
                            Ativo = true,
                            NomeArquivo = arq.arquivo,
                            Ordem = banner.posicao
                        };

                        arquivoService.AddArquivo(modelArq);
                    }

                    transaction.Complete();

                    return true;
                }   
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool SalvarBanner2(Banner banner, IFormFile file)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    var idPai = Guid.Empty;

                    if (banner.id == Guid.Empty)
                    {
                        idPai = IncluiBanner(banner);
                    }
                    else
                    {
                        idPai = EditarBanner(banner);
                    }

                    if (file != null)
                    {
                        if (!arquivoService.DelArquivoPorIdPaiTipo(idPai, "imgBannerMob")) return false;

                        var arq = arquivoService.SalvarArquivo(file, "imgBannerMob", "", idPai.ToString()).Result;

                        if (!arq.sucesso) return false;

                        var modelArq = new Arquivo
                        {
                            IdPai = idPai,
                            CodigoTipoArquivo = "imgBannerMob",
                            Caminho = arq.caminho,
                            Ativo = true,
                            NomeArquivo = arq.arquivo,
                            Ordem = banner.posicao
                        };

                        arquivoService.AddArquivo(modelArq);
                    }

                    transaction.Complete();

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Guid EditarBanner(Banner model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE [Banner]
                       SET [posicao] = @posicao
                          ,[texto1] = @texto1
                          ,[texto2] = @texto2
                          ,[texto3] = @texto3
                          ,[link] = @link
                          ,[cor] = @cor
                          ,[ativo] = @ativo
                    WHERE Id = @Id
                ", model);
            }

            return model.id;
        }

        private Guid IncluiBanner(Banner model)
        {
            model.id = Guid.NewGuid();

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    INSERT INTO [Banner]
                               ([id]
                               ,[modulo]
                               ,[componente]
                               ,[posicao]
                               ,[texto1]
                               ,[texto2]
                               ,[texto3]
                               ,[link]
                               ,[cor]
                               ,[ativo])
                         VALUES
                               (@id
                               ,@modulo
                               ,@componente
                               ,@posicao
                               ,@texto1
                               ,@texto2
                               ,@texto3
                               ,@link
                               ,@cor
                               ,@ativo)

               ", model);
            }

            return model.id;
        }
    }
}
