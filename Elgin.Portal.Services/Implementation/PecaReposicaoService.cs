using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.Transactions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Elgin.Portal.Services.Implementation
{
    public class PecaReposicaoService : AbstractService<PecaReposicaoService>
    {
        private ArquivoService arquivoService;

        public PecaReposicaoService(string connectionString, FileSettings fileSettings) : base(connectionString)
        {
            arquivoService = new ArquivoService(connectionString, fileSettings);
        }

        public List<PecaReposicao> ListarPorIdProduto(Guid idProduto)
        {
            var retorno = new List<PecaReposicao>();

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<PecaReposicao>(
                @"
                    SELECT [Id]
                          ,[Descricao]
                          ,[CodigoPecaReposicao]
                          ,[Referencia]
                          ,[Preco]
                          ,[IdProduto]
                          ,[Ativo]
                    FROM PecaReposicao
                    WHERE IdProduto = @idProduto
                ", new { idProduto }).ToList();
            }

        }

        public List<PecaReposicao> ListarPorCodigoProduto(string codigo)
        {

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<PecaReposicao>(
                @"
                    SELECT [Id]
                          ,[Descricao]
                          ,[CodigoPecaReposicao]
                          ,[Referencia]
                          ,[Preco]
                          ,[IdProduto]
                          ,[Ativo]
                    FROM PecaReposicao
                    WHERE IdProduto = (SELECT Id FROM Produto WHERE CodigoProduto = @codigo)
                          AND Ativo = 1
                ", new { codigo }).ToList();
            }

        }

        public Arquivo GetArquivo(Guid idProduto)
        {
            return arquivoService.GetArquivoPorIdPaiTipo(idProduto, "imgPecaReposicao");
        }

        public Arquivo GetArquivo(string codigo)
        {
            Guid idProduto;

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                idProduto = conexao.Query<Guid>(
                @"
                    SELECT [Id]
                    FROM Produto
                    WHERE CodigoProduto = @codigo
                ", new { codigo }).FirstOrDefault();
            }

            return arquivoService.GetArquivoPorIdPaiTipo(idProduto, "imgPecaReposicao");
        }

        public bool Salvar(List<PecaReposicao> pecasReposicao, Arquivo arquivoPeca, Guid idProduto)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    arquivoPeca.IdPai = idProduto;

                    if (arquivoPeca.Id == Guid.Empty)
                    {
                        var arquivoOld = arquivoService.GetArquivoPorIdPaiTipo(idProduto, "imgPecaReposicao");

                        if (arquivoOld != null)
                            arquivoService.DelArquivo(arquivoOld.Id);
                    }

                    arquivoService.AddArquivo(arquivoPeca);

                    foreach (var peca in pecasReposicao)
                    {
                        peca.IdProduto = idProduto;

                        if (peca.Id != Guid.Empty)
                        {
                            using (SqlConnection conexao = new SqlConnection(ConnectionString))
                            {
                                conexao.Execute(
                                @"
                                     UPDATE [PecaReposicao]
                                        SET [Descricao] = @Descricao
                                            ,[CodigoPecaReposicao] = @CodigoPecaReposicao
                                            ,[Referencia] = @Referencia
                                            ,[Preco] = @Preco
                                            ,[IdProduto] = @IdProduto
                                            ,[Ativo] = @Ativo
                                      WHERE Id = @Id
                                ", peca);
                            }
                        }

                        else
                        {
                            peca.Id = Guid.NewGuid();

                            using (SqlConnection conexao = new SqlConnection(ConnectionString))
                            {
                                conexao.Execute(
                                @"
                                     INSERT INTO [PecaReposicao]
                                                ([Id]
                                                ,[Descricao]
                                                ,[CodigoPecaReposicao]
                                                ,[Referencia]
                                                ,[Preco]
                                                ,[IdProduto]
                                                ,[Ativo])
                                            VALUES
                                                (@Id
                                                ,@Descricao
                                                ,@CodigoPecaReposicao
                                                ,@Referencia
                                                ,@Preco
                                                ,@IdProduto
                                                ,@Ativo)
                                ", peca);
                            }
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
