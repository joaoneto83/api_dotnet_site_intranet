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
    public class VideoService : AbstractService<VideoService>
    {

        public VideoService(string connectionString) : base(connectionString) { }

        public List<Video> ListarVideos(string modulo, int? idioma)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Video>(
                    @"
                    SELECT Video.Id ,
                           Video.LinkVideo,
                           CASE WHEN VideoIdioma.TituloVideo IS NULL OR VideoIdioma.TituloVideo = '' OR @idioma IS NULL THEN
                                Video.TituloVideo
	                       ELSE
		                        VideoIdioma.TituloVideo
                           END AS TituloVideo,
                           CASE WHEN VideoIdioma.DescricaoVideo IS NULL OR VideoIdioma.DescricaoVideo = '' OR @idioma IS NULL THEN
                                Video.DescricaoVideo
	                       ELSE
		                        VideoIdioma.DescricaoVideo
                           END AS DescricaoVideo,
	                       Modulo,
                           Ativo,
                           Ordem
                    FROM Video
                    LEFT JOIN VideoIdioma
                    ON Video.Id = VideoIdioma.IdVideo AND VideoIdioma.CodigoIdioma = @idioma
                    WHERE Modulo = @modulo
                    ORDER BY Ordem
                ", new { modulo, idioma }).ToList();
            }
        }

        public List<Video> Salvar(List<Video> videos)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    RemoverVideos(videos);

                    foreach (var model in videos)
                    {
                        if (model.Id == Guid.Empty)
                        {
                            model.Id = Guid.NewGuid();
                            IncluirVideo(model);
                        }
                        else
                        {
                            EditarVideo(model);
                        }

                    }

                    transaction.Complete();
                }
            }
            catch (Exception)
            {
                return null;
            }

            return videos;
        }

        private void RemoverVideos(List<Video> videos)
        {
            var modulos = videos.Select(x => x.Modulo).Distinct();
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [Video] WHERE Id NOT IN @ids AND Modulo IN @modulos
                ", new
                {
                    ids = videos.Select(x => x.Id).ToList(), modulos
                });
            }
        }

        private void EditarVideo(Video model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE [Video]
                       SET [Id] = @Id
                          ,[LinkVideo] = @LinkVideo
                          ,[TituloVideo] = @TituloVideo
                          ,[DescricaoVideo] = @DescricaoVideo
                          ,[Modulo] = @Modulo
                          ,[Ativo] = @Ativo
                          ,[Ordem] = @Ordem
                     WHERE Id = @Id
               ", model);
            }
        }

        private void IncluirVideo(Video model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    INSERT INTO [Video]
                               ([Id]
                               ,[LinkVideo]
                               ,[TituloVideo]
                               ,[DescricaoVideo]
                               ,[Modulo]
                               ,[Ativo]
                               ,[Ordem])
                         VALUES
                               (@Id
                               ,@LinkVideo
                               ,@TituloVideo
                               ,@DescricaoVideo
                               ,@Modulo
                               ,@Ativo
                               ,@Ordem)
               ", model);
            }
        }
    }
}
