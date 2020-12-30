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
    public class GrupoDestaqueService : AbstractService<GrupoDestaqueService>
    {

        public GrupoDestaqueService(string connectionString) : base(connectionString) { }

        public List<GrupoDestaque> ListarGrupos(string modulo, int? idioma)
        {
            var retorno = new List<GrupoDestaque>();

            var sql = @"
                    SELECT GrupoDestaque.Id
                          ,NomeGrupoDestaque
                          ,CodigoGrupoDestaque
	                      ,Produto.NomeProduto
                          ,Produto.CodigoProduto
              
                          ,CASE WHEN ProdutoIdioma.NomeProduto IS NULL OR ProdutoIdioma.NomeProduto = '' OR @idioma IS NULL THEN
		                      Produto.NomeProduto
	                       ELSE
		                      ProdutoIdioma.NomeProduto
	                       END as NomeProduto
                          ,Produto.Id
                    FROM GrupoDestaque
                    LEFT JOIN GrupoDestaqueProduto
                    ON GrupoDestaque.Id = GrupoDestaqueProduto.IdGrupoDestaque
                    LEFT JOIN Produto
                    ON GrupoDestaqueProduto.IdProduto = Produto.Id
                    LEFT JOIN ProdutoIdioma
                    ON Produto.Id = ProdutoIdioma.IdProduto AND ProdutoIdioma.CodigoIdioma = @idioma
                    WHERE GrupoDestaque.Ativo = 1 AND Modulo = @modulo
                    ORDER BY GrupoDestaque.Ordem
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, GrupoDestaque>();

                retorno = conexao.Query<GrupoDestaque, Produto, GrupoDestaque>(sql,
                (grupoDestaque, produto) =>
                {
                    GrupoDestaque grupoEntry;

                    if (!dictionary.TryGetValue(grupoDestaque.Id, out grupoEntry))
                    {
                        grupoEntry = grupoDestaque;
                        grupoEntry.Produtos = new List<Produto>();
                        dictionary.Add(grupoEntry.Id, grupoEntry);
                    }

                    if (produto != null && !grupoEntry.Produtos.Any(x => x.Id == produto.Id))
                        grupoEntry.Produtos.Add(produto);

                    return grupoEntry;

                }, new { modulo, idioma },
                splitOn: "NomeProduto, CodigoProduto").Distinct().ToList();
            }

            return retorno;
        }

        public bool Salvar(List<GrupoDestaque> grupos)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    foreach (var grupo in grupos)
                    {
                        RemoverGrupoDestaqueProduto(grupo);
                        SalvarGrupoDestaqueProduto(grupo);
                    }

                    transaction.Complete();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void RemoverGrupoDestaqueProduto(GrupoDestaque model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                       DELETE FROM GrupoDestaqueProduto WHERE IdGrupoDestaque = @Id

                ", model);
            }
        }

        private void SalvarGrupoDestaqueProduto(GrupoDestaque model)
        {
            foreach (var produto in model.Produtos)
            {
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Execute(
                    @"
                        INSERT INTO [GrupoDestaqueProduto]
                                   ([IdGrupoDestaque]
                                   ,[IdProduto])
                             VALUES
                                   (@idGrupoDestaque
                                   ,@idProduto)

                ", new { idProduto = produto.Id, idGrupoDestaque = model.Id });
                }
            }
        }

        public List<GrupoDestaque> RetornarGrupoDestaqueProdutos(string modulo, int idioma)
        {
            var gruposDestaque = ListarGrupos(modulo, idioma);

            foreach (var grupo in gruposDestaque)
            {


                foreach (var produto in grupo.Produtos)
                {
                    var sql = @"
                                SELECT Id
	                                  ,NomeArquivo
	                                  ,Caminho
                                FROM Arquivo
                                WHERE IdPai = @idProduto 
                                AND IdTipoArquivo = (SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'imgCard')
                                ORDER BY Ordem
                        ";

                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        produto.Arquivos = conexao.Query<Arquivo>(sql, new { idProduto = produto.Id }).ToList();
                    }
                }
            }

            return gruposDestaque;
        }

        public GrupoDestaque RetornarGrupoDestaquePorCodigo(string codigo, int idioma)
        {
            var grupoDestaque = GetGrupoPorCodigo(codigo, idioma);

            foreach (var produto in grupoDestaque.Produtos)
            {
                var sql = @"
                                SELECT Id 
	                                  ,NomeArquivo
	                                  ,Caminho
                                FROM Arquivo
                                WHERE IdPai = @idProduto 
                                AND IdTipoArquivo = (SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'imgCard')
                                ORDER BY Ordem
                        ";

                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    produto.Arquivos = conexao.Query<Arquivo>(sql, new { idProduto = produto.Id }).ToList();
                }
            }

            return grupoDestaque;
        }

        public GrupoDestaque GetGrupoPorCodigo(string codigo, int idioma)
        {
            var retorno = new GrupoDestaque();

            var sql = @"
                    SELECT GrupoDestaque.Id
                          ,NomeGrupoDestaque
                          ,CodigoGrupoDestaque
                    FROM GrupoDestaque
                    WHERE GrupoDestaque.Ativo = 1 AND CodigoGrupoDestaque = @codigo
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<GrupoDestaque>(sql, new { codigo }).FirstOrDefault();
            }

            var sqlProduto = @"
                    SELECT Produto.Id
                          ,CASE WHEN ProdutoIdioma.NomeProduto IS NULL OR ProdutoIdioma.NomeProduto = '' THEN
		                     Produto.NomeProduto
	                      ELSE
		                     ProdutoIdioma.NomeProduto
                          END AS NomeProduto
                    FROM Produto
                    LEFT JOIN GrupoDestaqueProduto
                    ON Produto.Id = GrupoDestaqueProduto.IdProduto
                    LEFT JOIN ProdutoIdioma
                    ON Produto.Id = ProdutoIdioma.IdProduto AND ProdutoIdioma.CodigoIdioma = @idioma
                    WHERE GrupoDestaqueProduto.IdGrupoDestaque = @idGrupo
                    ORDER BY Produto.NomeProduto
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno.Produtos = conexao.Query<Produto>(sqlProduto, new { idGrupo = retorno.Id, idioma }).ToList();
            }

            return retorno;
        }

        public LinksGrupoDestaque RetornarLinksGrupoDestaque()
        {
            var grupoDestaque = new LinksGrupoDestaque();
            var links = new List<GrupoDestaque>();

            var sql = @"
                    SELECT CodigoGrupoDestaque, Link
                    FROM GrupoDestaque
                    WHERE CodigoGrupoDestaque = 'RefrigeracaoIndustrial'
                          OR CodigoGrupoDestaque = 'LinhaComercialIntermediario'
                          OR CodigoGrupoDestaque = 'LinhaComercialLeve'
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                links = conexao.Query<GrupoDestaque>(sql).ToList();
                grupoDestaque.LinkRefrigeracaoLeve = links.FirstOrDefault(x => x.CodigoGrupoDestaque == "LinhaComercialLeve")?.Link;
                grupoDestaque.LinkRefrigeracaoComercial = links.FirstOrDefault(x => x.CodigoGrupoDestaque == "LinhaComercialIntermediario")?.Link;
                grupoDestaque.LinkRefrigeracaoIndustrial = links.FirstOrDefault(x => x.CodigoGrupoDestaque == "RefrigeracaoIndustrial")?.Link;
            }

            return grupoDestaque;
        }

        public bool SalvarLinksGrupoDestaque(LinksGrupoDestaque model)
        {
            try
            {
                if (!model.LinkRefrigeracaoLeve.Contains("http://") && !model.LinkRefrigeracaoLeve.Contains("https://"))
                    model.LinkRefrigeracaoLeve = "http://" + model.LinkRefrigeracaoLeve;

                if (!model.LinkRefrigeracaoComercial.Contains("http://") && !model.LinkRefrigeracaoComercial.Contains("https://"))
                    model.LinkRefrigeracaoComercial = "http://" + model.LinkRefrigeracaoComercial;

                if (!model.LinkRefrigeracaoIndustrial.Contains("http://") && !model.LinkRefrigeracaoIndustrial.Contains("https://"))
                    model.LinkRefrigeracaoIndustrial = "http://" + model.LinkRefrigeracaoIndustrial;


                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    var sql = $@"UPDATE [GrupoDestaque] SET [Link] = '{model.LinkRefrigeracaoLeve}'
                            WHERE [CodigoGrupoDestaque] = 'LinhaComercialLeve'";

                    sql += $@" UPDATE [GrupoDestaque] SET [Link] = '{model.LinkRefrigeracaoComercial}'
                            WHERE [CodigoGrupoDestaque] = 'LinhaComercialIntermediario'";

                    sql += $@" UPDATE [GrupoDestaque] SET [Link] = '{model.LinkRefrigeracaoIndustrial}'
                            WHERE [CodigoGrupoDestaque] = 'RefrigeracaoIndustrial'";

                    conexao.Execute(sql);
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
