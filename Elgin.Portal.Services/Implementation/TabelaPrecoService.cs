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
    public class TabelaPrecoService : AbstractService<TabelaPrecoService>
    {
        public TabelaPrecoService(string connectionString) : base(connectionString) { }

        public List<TabelaPreco> ListarTabelasPreco()
        {
            var retorno = new List<TabelaPreco>();

            var sql = @"
                    SELECT 
                        TabelaPreco.Id
	                    ,TabelaPreco.NomeTabelaPreco
                        ,TabelaPreco.Ativo
                        ,Arquivo.Caminho
                        ,Arquivo.Id
                        ,Arquivo.NomeArquivo
                        ,Arquivo.Ordem
                        ,Arquivo.Ativo
                    FROM TabelaPreco
                    LEFT JOIN Arquivo
                    ON Arquivo.IdPai = TabelaPreco.Id
                    WHERE TabelaPreco.Ativo = 1
                    ORDER BY
                    NomeTabelaPreco, Arquivo.Ordem
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, TabelaPreco>();

                retorno = conexao.Query<TabelaPreco, Arquivo, TabelaPreco>(sql,
                (tabela, arquivo) =>
                {
                    TabelaPreco tabelaEntry;

                    if (!dictionary.TryGetValue(tabela.Id, out tabelaEntry))
                    {
                        tabelaEntry = tabela;
                        tabelaEntry.Arquivos = new List<Arquivo>();
                        dictionary.Add(tabelaEntry.Id, tabelaEntry);
                    }

                    if (arquivo != null)
                        tabelaEntry.Arquivos.Add(arquivo);

                    return tabelaEntry;

                },
                splitOn: "Caminho").Distinct().ToList();
            }

            return retorno;
        }

        public bool MoverItem(Guid id, Guid idPastaSelecionada, bool isTabela)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    if (isTabela)
                    {
                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                                 UPDATE TabelaPreco SET IdPastaTabelaPreco = @idPastaSelecionada
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
                                 UPDATE PastaTabelaPreco SET IdPai = @idPastaSelecionada
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

        public ResponsePastaTabelaPreco ListarPastaTabelaPreco(Guid? id)
        {
            var retorno = new ResponsePastaTabelaPreco();

            if (id != null && id != Guid.Empty)
            {
                var sql = @"
                    SELECT 
                        Id
	                    ,Nome
                        ,IdPai
                    FROM PastaTabelaPreco
                    WHERE Id = @id AND Ativo = 1
                    ORDER BY Nome
                ";

                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    retorno = conexao.Query<ResponsePastaTabelaPreco>(sql, new { id }).FirstOrDefault();
                }

                if (retorno.IdPai == null)
                    retorno.IdPai = Guid.Empty;

                retorno.Pastas = GetPastaFilhos(retorno.Id);
                retorno.TabelaPrecos = GetTabelasPrecoPorPasta(retorno.Id);
            }
            else
            {
                retorno.Pastas = GetPastaFilhos(null);
                retorno.TabelaPrecos = GetTabelasPrecoPorPasta(null);
            }

            return retorno;
        }

        public List<PastaTabelaPreco> ListarAllPastas()
        {
            var retorno = new List<PastaTabelaPreco>();

            var sql = @"
                    SELECT 
                        Id
	                    ,Nome
                        ,IdPai
                    FROM PastaTabelaPreco
                    WHERE Ativo = 1
                    ORDER BY Nome
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<PastaTabelaPreco>(sql).ToList();
            }

            return retorno;
        }

        private List<TabelaPreco> GetTabelasPrecoPorPasta(Guid? idPasta)
        {
            var retorno = new List<TabelaPreco>();
            var sql = string.Empty;

            if (idPasta != null)
            {
                sql = @"
                    SELECT 
                        TabelaPreco.Id
	                    ,TabelaPreco.NomeTabelaPreco
                        ,TabelaPreco.Ativo
                        ,Arquivo.Caminho
                        ,Arquivo.Id
                        ,Arquivo.NomeArquivo
                        ,Arquivo.Ordem
                        ,Arquivo.Ativo
                    FROM TabelaPreco
                    LEFT JOIN Arquivo
                    ON Arquivo.IdPai = TabelaPreco.Id
                    WHERE TabelaPreco.Ativo = 1 AND IdPastaTabelaPreco = @idPasta
                    ORDER BY
                    NomeTabelaPreco, Arquivo.Ordem
                ";
            }
            else
            {
                sql = @"
                    SELECT 
                        TabelaPreco.Id
	                    ,TabelaPreco.NomeTabelaPreco
                        ,TabelaPreco.Ativo
                        ,Arquivo.Caminho
                        ,Arquivo.Id
                        ,Arquivo.NomeArquivo
                        ,Arquivo.Ordem
                        ,Arquivo.Ativo
                    FROM TabelaPreco
                    LEFT JOIN Arquivo
                    ON Arquivo.IdPai = TabelaPreco.Id
                    WHERE TabelaPreco.Ativo = 1 AND IdPastaTabelaPreco IS NULL
                    ORDER BY
                    NomeTabelaPreco, Arquivo.Ordem
                ";
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, TabelaPreco>();

                retorno = conexao.Query<TabelaPreco, Arquivo, TabelaPreco>(sql,
                (tabela, arquivo) =>
                {
                    TabelaPreco tabelaEntry;

                    if (!dictionary.TryGetValue(tabela.Id, out tabelaEntry))
                    {
                        tabelaEntry = tabela;
                        tabelaEntry.Arquivos = new List<Arquivo>();
                        dictionary.Add(tabelaEntry.Id, tabelaEntry);
                    }

                    if (arquivo != null)
                        tabelaEntry.Arquivos.Add(arquivo);

                    return tabelaEntry;

                }, new { idPasta },
                splitOn: "Caminho").Distinct().ToList();
            }

            return retorno;
        }

        private List<PastaTabelaPreco> GetPastaFilhos(Guid? idPai)
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
                    FROM PastaTabelaPreco
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
                    FROM PastaTabelaPreco
                    WHERE IdPai IS NULL AND Ativo = 1
                    ORDER BY Nome
                ";
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<PastaTabelaPreco>(sql, new { idPai }).ToList();
            }
        }

        public bool Salvar(TabelaPreco tabela)
        {

            try
            {
                using (var transaction = new TransactionScope())
                {
                    if (tabela.Id != Guid.Empty)
                    {
                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                                 UPDATE TabelaPreco SET NomeTabelaPreco = @NomeTabelaPreco, Ativo = @Ativo
                                 WHERE Id = @Id
                            ", tabela);
                        }
                    }

                    else
                    {
                        tabela.Id = Guid.NewGuid();

                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                                 INSERT INTO [dbo].[TabelaPreco]
                                       ([Id]
                                       ,[NomeTabelaPreco]
                                       ,[Ativo]
                                       ,[IdPastaTabelaPreco])
                                 VALUES
                                       (@Id
                                       ,@NomeTabelaPreco
		                               ,1
                                       ,@IdPastaTabelaPreco)
                            ", tabela);
                        }
                    }

                    foreach (var item in tabela.Arquivos)
                    {
                        item.NomeArquivo = Path.GetFileNameWithoutExtension(item.Caminho).Replace("%20", " ").Split("_").Last();
                        item.IdPai = tabela.Id;
                    }

                    RemoverArquivos(tabela);
                    SalvarArquivosTabelaPreco(tabela);

                    transaction.Complete();
                }
            }
        catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool SalvarPasta(PastaTabelaPreco pasta)
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
                                 UPDATE PastaTabelaPreco SET Nome = @Nome, Ativo = @Ativo
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
                                 INSERT INTO [dbo].[PastaTabelaPreco]
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

        private void RemoverArquivos(TabelaPreco model)
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

        private void SalvarArquivosTabelaPreco(TabelaPreco model)
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
                                       ((SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'tabelaPreco')
                                       ,@NomeArquivo
                                       ,@Caminho
                                       ,1
                                       ,@IdPai)
                ", model.Arquivos.Where(x => x.Id == Guid.Empty));
            }
        }
    }
}
