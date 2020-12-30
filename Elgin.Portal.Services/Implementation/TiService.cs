using Dapper;
using Elgin.Portal.Services.Implementation.Shared;
using Elgin.Portal.Services.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;

namespace Elgin.Portal.Services.Implementation
{
    public class TiService : AbstractService<TiService>
    {
        public TiService(string connectionString) : base(connectionString) { }

        public List<ModuloArquivoTi> ListarModuloArquivo()
        {
            var retorno = new List<ModuloArquivoTi>();

            var sql = @"
                    SELECT 
                        ModuloArquivoTi.Id
	                    ,ModuloArquivoTi.Nome
                        ,ModuloArquivoTi.Secao
                        ,ModuloArquivoTi.Ativo
                        ,Arquivo.Caminho
                        ,Arquivo.Id
                        ,Arquivo.NomeArquivo
                        ,Arquivo.Ordem
                        ,Arquivo.Ativo
                    FROM ModuloArquivoTi
                    LEFT JOIN Arquivo
                    ON Arquivo.IdPai = ModuloArquivoTi.Id
                    WHERE ModuloArquivoTi.Ativo = 1
                    ORDER BY
                    ModuloArquivoTi.Nome, Arquivo.Ordem
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, ModuloArquivoTi>();

                retorno = conexao.Query<ModuloArquivoTi, Arquivo, ModuloArquivoTi>(sql,
                (tabela, arquivo) =>
                {
                    ModuloArquivoTi tabelaEntry;

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

        public bool SalvarModulo(ModuloArquivoTi modulo)
        {

            try
            {
                using (var transaction = new TransactionScope())
                {
                    if (modulo.Id != Guid.Empty)
                    {
                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                                 UPDATE ModuloArquivoTi SET Nome = @Nome, Ativo = @Ativo
                                 WHERE Id = @Id
                            ", modulo);
                        }
                    }

                    else
                    {
                        modulo.Id = Guid.NewGuid();

                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                                 INSERT INTO [dbo].[ModuloArquivoTi]
                                       ([Id]
                                       ,[Nome]
                                       ,[Secao]
                                       ,[Ativo])
                                 VALUES
                                       (@Id
                                       ,@Nome
                                       ,@Secao
		                               ,1)
                            ", modulo);
                        }
                    }

                    foreach (var item in modulo.Arquivos)
                    {
                        item.NomeArquivo = Path.GetFileNameWithoutExtension(item.Caminho).Replace("%20", " ").Split("_").Last();
                        item.IdPai = modulo.Id;
                    }

                    RemoverArquivos(modulo);
                    SalvarArquivosModuloTi(modulo);

                    transaction.Complete();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void RemoverArquivos(ModuloArquivoTi model)
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

        private void SalvarArquivosModuloTi(ModuloArquivoTi model)
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
                                       ((SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'ti')
                                       ,@NomeArquivo
                                       ,@Caminho
                                       ,1
                                       ,@IdPai)
                ", model.Arquivos.Where(x => x.Id == Guid.Empty));
            }
        }
    }
}
