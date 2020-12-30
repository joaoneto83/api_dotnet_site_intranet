using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.Transactions;
using System.IO;

namespace Elgin.Portal.Services.Implementation
{
    public class MerchandisingService : AbstractService<MerchandisingService>
    {
        private ImagemService imagemService;

        public MerchandisingService(string connectionString) : base(connectionString)
        {

            imagemService = new ImagemService(connectionString);

        }

        public Merchandising GetMerchandising()
        {
            var model = new Merchandising();

            model.Banners = imagemService.Banners("Intranet-Merchandising", "1", null);

            return model;
        }

        public List<Arquivo> ListaArquivosMpdv()
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
                                  ,TipoArquivo.CodigoTipoArquivo
                              FROM [Arquivo]
                            LEFT JOIN TipoArquivo
                            ON TipoArquivo.Id = Arquivo.IdTipoArquivo
                            WHERE TipoArquivo.CodigoTipoArquivo = 'merchanSazonal'
                               OR TipoArquivo.CodigoTipoArquivo = 'merchanInstitucional'
                               OR TipoArquivo.CodigoTipoArquivo = 'merchanLinhaProdutos'
                               OR TipoArquivo.CodigoTipoArquivo = 'merchanExpositores'
                ";



            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Arquivo>(sql, new { }).ToList();
            }
        }

        public ResponsePastaTreinamentoMerchan ListarPastaTreinamento(Guid? id)
        {
            var retorno = new ResponsePastaTreinamentoMerchan();

            if (id != null && id != Guid.Empty)
            {
                var sql = @"
                    SELECT 
                        Id
	                    ,Nome
                        ,IdPai
                    FROM PastaTreinamentoMerchan
                    WHERE Id = @id AND Ativo = 1
                    ORDER BY Nome
                ";

                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    retorno = conexao.Query<ResponsePastaTreinamentoMerchan>(sql, new { id }).FirstOrDefault();
                }

                if (retorno.IdPai == null)
                    retorno.IdPai = Guid.Empty;

                retorno.Pastas = GetPastaFilhos(retorno.Id);
                retorno.Treinamentos = GetTreinamentosPorPasta(retorno.Id);
            }
            else
            {
                retorno.Pastas = GetPastaFilhos(null);
                retorno.Treinamentos = GetTreinamentosPorPasta(null);
            }

            return retorno;
        }

        private List<TreinamentoMerchan> GetTreinamentosPorPasta(object id)
        {
            throw new NotImplementedException();
        }

        private List<PastaTreinamentoMerchan> GetPastaFilhos(object id)
        {
            throw new NotImplementedException();
        }

        private List<TreinamentoMerchan> GetTreinamentosPorPasta(Guid? idPasta)
        {
            var retorno = new List<TreinamentoMerchan>();
            var sql = string.Empty;

            if (idPasta != null)
            {
                sql = @"
                    SELECT 
                        TreinamentoMerchan.Id
	                    ,TreinamentoMerchan.Nome
                        ,TreinamentoMerchan.Ativo
                        ,Arquivo.Caminho
                        ,Arquivo.Id
                        ,Arquivo.NomeArquivo
                        ,Arquivo.Ordem
                        ,Arquivo.Ativo
                    FROM TreinamentoMerchan
                    LEFT JOIN Arquivo
                    ON Arquivo.IdPai = TreinamentoMerchan.Id
                    WHERE TreinamentoMerchan.Ativo = 1 AND IdPastaTreinamentoMerchan = @idPasta
                    ORDER BY
                    Nome, Arquivo.Ordem
                ";
            }
            else
            {
                sql = @"
                    SELECT 
                        TreinamentoMerchan.Id
	                    ,TreinamentoMerchan.Nome
                        ,TreinamentoMerchan.Ativo
                        ,Arquivo.Caminho
                        ,Arquivo.Id
                        ,Arquivo.NomeArquivo
                        ,Arquivo.Ordem
                        ,Arquivo.Ativo
                    FROM TreinamentoMerchan
                    LEFT JOIN Arquivo
                    ON Arquivo.IdPai = TreinamentoMerchan.Id
                    WHERE TreinamentoMerchan.Ativo = 1 AND IdPastaTreinamentoMerchan IS NULL
                    ORDER BY
                    Nome, Arquivo.Ordem
                ";
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, TreinamentoMerchan>();

                retorno = conexao.Query<TreinamentoMerchan, Arquivo, TreinamentoMerchan>(sql,
                (treinamento, arquivo) =>
                {
                    TreinamentoMerchan treinamentoEntry;

                    if (!dictionary.TryGetValue(treinamento.Id, out treinamentoEntry))
                    {
                        treinamentoEntry = treinamento;
                        treinamentoEntry.Arquivos = new List<Arquivo>();
                        dictionary.Add(treinamentoEntry.Id, treinamentoEntry);
                    }

                    if (arquivo != null)
                        treinamentoEntry.Arquivos.Add(arquivo);

                    return treinamentoEntry;

                }, new { idPasta },
                splitOn: "Caminho").Distinct().ToList();
            }

            return retorno;
        }

        private List<PastaTreinamentoMerchan> GetPastaFilhos(Guid? idPai)
        {
            var sql = string.Empty;

            if (idPai != null)
            {
                sql = @"
                    SELECT 
                        Id
	                    ,Nome
                        ,IdPai
                        ,Ativo
                    FROM PastaTreinamentoMerchan
                    WHERE IdPai = @idPai AND Ativo = 1
                    ORDER BY Nome
                ";
            }
            else
            {
                sql = @"
                    SELECT 
                        Id
	                    ,Nome
                        ,IdPai
                        ,Ativo
                    FROM PastaTreinamentoMerchan
                    WHERE IdPai IS NULL AND Ativo = 1
                    ORDER BY Nome
                ";
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<PastaTreinamentoMerchan>(sql, new { idPai }).ToList();
            }
        }

        public bool SalvarPasta(PastaTreinamentoMerchan pasta)
        {

            try
            {
                using (var transaction = new TransactionScope())
                {
                    if (pasta.Id != Guid.Empty)
                    {
                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                                 UPDATE PastaTreinamentoMerchan SET Nome = @Nome, Ativo = @Ativo
                                 WHERE Id = @Id
                            ", pasta);
                        }
                    }

                    else
                    {
                        pasta.Id = Guid.NewGuid();

                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                                 INSERT INTO [dbo].[PastaTreinamentoMerchan]
                                       ([Id]
                                       ,[Nome]
                                       ,[IdPai]
                                       ,[Ativo])
                                 VALUES
                                       (@Id
                                       ,@Nome
                                       ,@IdPai
		                               ,1)
                            ", pasta);
                        }
                    }

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public bool SalvarTreinamento(TreinamentoMerchan model)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    if (model.Id != Guid.Empty)
                    {
                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                                 UPDATE TreinamentoMerchan SET Nome = @Nome, Ativo = @Ativo
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
                                 INSERT INTO [dbo].[TreinamentoMerchan]
                                       ([Id]
                                       ,[Nome]
                                       ,[Ativo]
                                       ,[IdPastaTreinamentoMerchan])
                                 VALUES
                                       (@Id
                                       ,@Nome
		                               ,1
                                       ,@IdPastaTreinamentoMerchan)
                            ", model);
                        }
                    }

                    foreach (var item in model.Arquivos)
                    {
                        item.NomeArquivo = Path.GetFileNameWithoutExtension(item.Caminho).Replace("%20", " ").Split("_").Last();
                        item.IdPai = model.Id;
                    }

                    RemoverArquivos(model);
                    SalvarArquivosTreinamento(model);

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private void SalvarArquivosTreinamento(TreinamentoMerchan model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        INSERT INTO [Arquivo]
                                       ([IdTipoArquivo]
                                       ,[NomeArquivo]
                                       ,[Caminho]
                                       ,[Ativo]
                                       ,[IdPai])
                                 VALUES
                                       ((SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'treinamentosMerchan')
                                       ,@NomeArquivo
                                       ,@Caminho
                                       ,1
                                       ,@IdPai)
                ", model.Arquivos.Where(x => x.Id == Guid.Empty));
            }
        }

        private void RemoverArquivos(TreinamentoMerchan model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [Arquivo]
                         WHERE Id NOT IN @Ids AND IdPai = @IdPai
                ", new
                {
                    Ids = model.Arquivos.Select(x => x.Id).ToList(),
                    IdPai = model.Id
                });
            }
        }

        public List<PastaTabelaPreco> ListarAllPastas()
        {
            var retorno = new List<PastaTabelaPreco>();

            var sql = @"
                    SELECT 
                        Id
	                    ,Nome
                        ,IdPai
                    FROM PastaTreinamentoMerchan
                    WHERE Ativo = 1
                    ORDER BY Nome
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<PastaTabelaPreco>(sql).ToList();
            }

            return retorno;
        }

        public bool MoverItem(Guid id, Guid idPastaSelecionada, bool isTreinamento)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    if (isTreinamento)
                    {
                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                                 UPDATE TreinamentoMerchan SET IdPastaTreinamentoMerchan = @idPastaSelecionada
                                 WHERE Id = @id
                            ", new { id, idPastaSelecionada });
                        }
                    }

                    else
                    {
                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                                 UPDATE PastaTreinamentoMerchan SET IdPai = @idPastaSelecionada
                                 WHERE Id = @id
                            ", new { id, idPastaSelecionada });
                        }
                    }

                    transaction.Complete();
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
