using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Dapper;
using Elgin.Portal.Services.Implementation.Shared;
using Elgin.Portal.Services.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;

namespace Elgin.Portal.Services.Implementation
{
    public class ArquivoService : AbstractService<ArquivoService>
    {
        public ArquivoService(string connectionString, FileSettings fileSettings) : base(connectionString, fileSettings)
        {

        }

        public Arquivo AddArquivo(Arquivo model)
        {
            if (model.Id != Guid.Empty)
            {
                UpdateArquivo(model);
            }
            else
            {
                InsertArquivo(model);
            }

            return model;
        }

        public List<Arquivo> RetornaTodos()
        {
            var sql = @"
                            SELECT [Arquivo].[Id]
                                  ,[Arquivo].[IdTipoArquivo]
                                  ,[Arquivo].[NomeArquivo]
                                  ,[Arquivo].[Caminho]
                                  ,[Arquivo].[Ordem]
                                  ,[Arquivo].[Ativo]
                                  ,[Arquivo].[IdPai]
                                  ,[Arquivo].[IdSecao]
                                  ,TIpoArquivo.CodigoTipoArquivo
                              FROM [Arquivo]
                            LEFT JOIN TipoArquivo
                            ON TIpoArquivo.Id = Arquivo.IdTipoArquivo
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Arquivo>(sql, new { }).ToList();
            }
        }

        public void UpdateArquivo(Arquivo model)
        {
            var sql = @"
                            UPDATE [Arquivo]
                                SET [NomeArquivo] = @NomeArquivo
                                    ,[Caminho] = @Caminho
                                    ,[Ordem] = @Ordem
                            WHERE Id = @Id
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(sql, model);
            }
        }

        public List<TipoArquivo> RetornaTipoArquivos()
        {
            var sql = @"
                        SELECT [Id]
                              ,[NomeTipoArquivo]
                              ,[CodigoTipoArquivo]
                              ,[Ativo]
                          FROM [TipoArquivo]

                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<TipoArquivo>(sql, new {  }).ToList();
            }
        }

        public void InsertArquivo(Arquivo model)
        {
            model.Id = Guid.NewGuid();

            var sql = @"
                            INSERT INTO [Arquivo]
                                       ([Id]
                                       ,[IdTipoArquivo]
                                       ,[NomeArquivo]
                                       ,[Caminho]
                                       ,[Ordem]
                                       ,[Ativo]
                                       ,[IdPai]
                                       ,[IdSecao])
                                 VALUES
                                       (@Id
                                       ,(SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = @CodigoTipoArquivo)
                                       ,@NomeArquivo
                                       ,@Caminho
                                       ,@Ordem
                                       ,@Ativo
                                       ,@IdPai
                                       ,@IdSecao)
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(sql, model);
            }
        }

        public void InsertArquivo(List<Arquivo> model)
        {
            var sql = @"
                            INSERT INTO [Arquivo]
                                       ([IdTipoArquivo]
                                       ,[NomeArquivo]
                                       ,[Caminho]
                                       ,[Ordem]
                                       ,[Ativo]
                                       ,[IdPai]
                                       ,[IdSecao])
                                 VALUES
                                       ((SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = @CodigoTipoArquivo)
                                       ,@NomeArquivo
                                       ,@Caminho
                                       ,@Ordem
                                       ,@Ativo
                                       ,@IdPai
                                       ,@IdSecao)
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(sql, model);
            }
        }

        public void UpdateArquivo(List<Arquivo> model)
        {
            var sql = @"
                            UPDATE [Arquivo]
                                SET [NomeArquivo] = @NomeArquivo
                                    ,[Caminho] = @Caminho
                                    ,[IdPai] = @IdPai       
                                    ,[Ordem] = @Ordem
                            WHERE Id = @Id
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(sql, model);
            }
        }

        public string RetornarCaminhoArquivo(Guid id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<string>(
                @"SELECT Caminho FROM Arquivo WHERE Id = @Id", new { id }).First();
            }
        }

        public bool DelArquivo(Guid id)
        {
            var arquivo = GetArquivo(id);

            //RemoverArquivo(arquivo.Caminho);

            var sql = @"
                            DELETE FROM Arquivo    
                            WHERE Id = @id
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(sql, new { id });
            }

            return true;
        }

        public bool DelArquivoPorIdPai(Guid idPai)
        {

            var sql = @"
                            DELETE FROM Arquivo    
                            WHERE IdPai = @idPai
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(sql, new { idPai });
            }

            return true;
        }
        public bool DelArquivoPorIdPaiTipo(Guid idPai, string codigoTipoArquivo)
        {

            var sql = @"
                            DELETE FROM Arquivo    
                            WHERE IdPai = @IdPai
                            AND IdTipoArquivo = (SELECT Id from TipoArquivo WHERE CodigoTipoArquivo  = @CodigoTipoArquivo)
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(sql, new { IdPai = idPai, CodigoTipoArquivo = codigoTipoArquivo });
            }

            return true;
        }

        public bool DelArquivos(List<Guid> idsArquivo, Guid idPai, string codigoTipo)
        {
            var sql = @"
                            DELETE FROM Arquivo    
                            WHERE 
                            IdPai = @idPai AND 
                            ID NOT IN @idsArquivo AND
                            IdTipoArquivo = (SELECT ID FROM TipoArquivo WHERE CodigoTipoArquivo = @codigoTipo)
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(sql, new { idsArquivo, idPai, codigoTipo });
            }

            return true;
        }

        public Arquivo GetArquivo(Guid id)
        {
            var sql = @"
                    SELECT [Id]
                          ,[NomeArquivo]
                          ,[Caminho]
                          ,[Ordem]
                          ,[Ativo]
                          ,[IdPai]
                          ,[IdSecao]
                      FROM [Arquivo]
                    WHERE Id = @id
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Arquivo>(sql, new { id }).FirstOrDefault();
            }
        }

        public Arquivo GetArquivoPorIdPai(Guid idPai)
        {
            var sql = @"
                    SELECT [Id]
                          ,[NomeArquivo]
                          ,[Caminho]
                          ,[Ordem]
                          ,[Ativo]
                          ,[IdPai]
                          ,[IdSecao]
                      FROM [Arquivo]
                    WHERE IdPai = @idPai
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Arquivo>(sql, new { idPai }).FirstOrDefault();
            }
        }

        public Arquivo GetArquivoPorIdPaiTipo(Guid idPai, string codigoTipoArquivo)
        {
            var sql = @"
                    SELECT [Id]
                          ,[NomeArquivo]
                          ,[Caminho]
                          ,[Ordem]
                          ,[Ativo]
                          ,[IdPai]
                          ,[IdSecao]
                          ,[IdTipoArquivo]
                      FROM [Arquivo]
                    WHERE IdPai = @idPai
                    AND IdTipoArquivo = (SELECT Id from TipoArquivo WHERE CodigoTipoArquivo  = @codigoTipoArquivo)
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Arquivo>(sql, new { idPai, codigoTipoArquivo }).FirstOrDefault();
            }
        }

        public List<Arquivo> GetArquivoSecao(Guid idSecao)
        {
            var sql = @"
                    SELECT [Id]
                          ,[NomeArquivo]
                          ,[Caminho]
                          ,[Ordem]
                          ,[Ativo]
                          ,[IdPai]
                          ,[IdSecao]
                      FROM [Arquivo]
                    WHERE IdSecao = @IdSecao
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Arquivo>(sql, new { idSecao }).ToList();
            }
        }

        public List<Arquivo> RetornaArquivosProduto(Guid id, bool? ativo, int? idioma)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Arquivo>(
                @"
                    SELECT Arquivo.[Id]
                          ,TipoArquivo.[CodigoTipoArquivo]
                          ,CASE WHEN ArquivoIdioma.NomeArquivo IS NULL OR ArquivoIdioma.NomeArquivo = '' THEN
		                    Arquivo.NomeArquivo
	                      ELSE
		                    ArquivoIdioma.NomeArquivo
                          END AS NomeArquivo
                          ,[Caminho]
                          ,[Ordem]
                          ,Arquivo.[Ativo]
                          ,[IdPai]
                          ,[IdSecao]
                      FROM [dbo].[Arquivo]
                      JOIN TipoArquivo 
                        ON TipoArquivo.Id = Arquivo.IdTipoArquivo
                      LEFT JOIN ArquivoIdioma 
                        ON  Arquivo.Id = ArquivoIdioma.IdArquivo AND ArquivoIdioma.CodigoIdioma = @idioma
                    WHERE IdPai = @id AND (@ativo IS NULL OR Arquivo.Ativo = @ativo)
                    ORDER BY TipoArquivo.[CodigoTipoArquivo], Ordem
                ", new { id, ativo, idioma }).ToList();
            }
        }

        public Guid GetIdPorCodigo(string codigo)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Guid>(
                @"
                    SELECT [Id]
                      FROM [dbo].[TipoArquivo]
                    WHERE CodigoTipoArquivo = @codigo AND Ativo = 1
                ", new { codigo }).FirstOrDefault();
            };
        }

        private async void removeBlobAsync(string caminho)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(FileSettings.BlobConnection);

                var bobClient = storageAccount.CreateCloudBlobClient();

                var container = bobClient.GetContainerReference("elginfiles");

                var caminhoSplit = caminho.Split('/');
                var arquivo = caminhoSplit[caminhoSplit.Length - 1];

                var blockBob = container.GetBlobReference(arquivo);

                await blockBob.DeleteAsync();
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<FileResponse> salvarBlobAsync(IFormFile file, string caminho, string nomeArquivo, string idPai)
        {
            try
            {
                var fileName = file.FileName;

                var storageAccount = CloudStorageAccount.Parse(FileSettings.BlobConnection);

                var bobClient = storageAccount.CreateCloudBlobClient();

                var container = bobClient.GetContainerReference("elginfiles");

                if (!string.IsNullOrEmpty(idPai))
                {
                    fileName = idPai + "_" + fileName;
                }

                if (!string.IsNullOrEmpty(caminho))
                {
                    fileName = caminho + "_" + fileName;
                }

                if (!string.IsNullOrEmpty(nomeArquivo))
                {
                    var extension = file.FileName.Split('.');
                    fileName = nomeArquivo + "." + extension[extension.Length - 1];
                }

                var blockBob = container.GetBlockBlobReference(fileName);

                using (var fileStream = file.OpenReadStream())
                {
                    await blockBob.UploadFromStreamAsync(fileStream);
                }

                return new FileResponse
                {
                    arquivo = blockBob.Name,
                    caminho = blockBob.Uri.AbsoluteUri,
                    sucesso = true
                };
            }
            catch (Exception ex)
            {
                return new FileResponse
                {
                    erro = ex.Message,
                    sucesso = false
                };

            }
        }

        public async Task<FileResponse> SalvarArquivo(IFormFile file, string caminho, string nomeArquivo, string idPai)
        {
            if(FileSettings.Type == FileSettings.FyleType.Blob)
            {
                return await salvarBlobAsync(file, caminho, nomeArquivo, idPai);
            }

            var serverPath = FileSettings.ServerPath;

            var retorno = new FileResponse();

            try
            {
                var fileName = file.FileName;

                if (!string.IsNullOrEmpty(idPai))
                {
                    fileName = idPai + "_" + fileName;
                }

                if (!string.IsNullOrEmpty(caminho))
                {
                    fileName = caminho + "_" + fileName;
                }

                if (!string.IsNullOrEmpty(nomeArquivo))
                {
                    var extension = file.FileName.Split('.');
                    fileName = nomeArquivo + "." + extension[extension.Length - 1];
                }

                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }

                if (file.Length > 0)
                {
                    var contentD = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                    var finalPath = Path.Combine(serverPath, fileName);

                    using (var stream = new FileStream(finalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var webPath = $"{FileSettings.WebPath}/{fileName}";

                    return new FileResponse
                    {
                        arquivo = file.Name,
                        caminho = webPath,
                        sucesso = true
                    };
                }

                return retorno;
            }
            catch (Exception ex)
            {
                return new FileResponse
                {
                    erro = ex.Message,
                    sucesso = false
                };
            }
        }

        public void RemoverArquivo(string caminho)
        {
            if(FileSettings.Type == FileSettings.FyleType.Blob)
            {
                removeBlobAsync(caminho);
                return;
            }

            caminho = caminho.Replace(FileSettings.WebPath, FileSettings.ServerPath);

            File.Delete(caminho);
        }

    }
}
