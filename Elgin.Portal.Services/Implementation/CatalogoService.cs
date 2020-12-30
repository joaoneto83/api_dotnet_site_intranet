using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.Transactions;
using System.IO;

namespace Elgin.Portal.Services.Implementation
{
    public class CatalogoService : AbstractService<CatalogoService>
    {
        private DominioService dominioService;
        private ArquivoService arquivoService;
        private EspecificacaoTecnicaService especService;

        public CatalogoService(string connectionString, FileSettings fileSettings) : base(connectionString)
        {
            dominioService = new DominioService(connectionString);
            arquivoService = new ArquivoService(connectionString, fileSettings);
            especService = new EspecificacaoTecnicaService(connectionString);
        }

        public List<Catalogo> RetornaCatalogo()
        {
            var retorno = new List<Catalogo>();

            var sql = @"
                    SELECT 
	                    Linha.Id as IdLinha,
	                    Linha.NomeLinha AS NomeLinha,
                        Linha.Cor1,
	                    Linha.Cor2,
	                    Linha.CorTitulo,
	                    CatalogoLinha.Id AS IdArquivoLinha,
	                    CatalogoLinha.Caminho AS UrlCatalogoLinha,
	                    CatalogoLinha.NomeArquivo AS NomeArquivoLinha,
	                    SubLinha.NomeSubLinha,
	                    SubLinha.Id,
	                    SubLinha.IdLinha,
	                    Produto.NomeProduto,
	                    Produto.Id,
	                    Produto.IdSubLinha,
	                    CatalogoProduto.NomeArquivo,
	                    CatalogoProduto.Caminho,
	                    CatalogoProduto.Id,
	                    CatalogoProduto.IdPai
                    FROM Linha
                    LEFT JOIN SubLinha
                    ON Sublinha.IdLinha = Linha.Id AND SubLinha.Ativo = 1
                    LEFT JOIN Produto
                    ON Produto.IdSubLinha = SubLinha.Id AND Produto.Ativo = 1
                    LEFT JOIN Arquivo CatalogoLinha
                    ON CatalogoLinha.IdPai = Linha.Id AND CatalogoLinha.IdTipoArquivo = (SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'catalogoLinhaCompleta')
                    LEFT JOIN Arquivo CatalogoProduto
                    ON CatalogoProduto.IdPai = Produto.Id AND CatalogoProduto.IdTipoArquivo = (SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'catalogoProduto')
                    WHERE Linha.Ativo = 1
                    ORDER BY Linha.Ordem, SubLinha.NomeSubLinha, Produto.NomeProduto
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, Catalogo>();

                retorno = conexao.Query<Catalogo, SubLinha, Produto, Arquivo, Catalogo>(sql,
                (catalogo, sublinha, produto, arquivo) =>
                {
                    Catalogo catalogoEntry;

                    if (!dictionary.TryGetValue(catalogo.IdLinha, out catalogoEntry))
                    {
                        catalogoEntry = catalogo;
                        catalogoEntry.SubLinhas = new List<SubLinha>();
                        catalogoEntry.Produtos = new List<Produto>();
                        catalogoEntry.Arquivos = new List<Arquivo>();
                        dictionary.Add(catalogoEntry.IdLinha, catalogoEntry);
                    }

                    if (sublinha != null && !catalogoEntry.SubLinhas.Any(x => x.Id == sublinha.Id))
                    {
                        catalogoEntry.SubLinhas.Add(sublinha);
                    }

                    if (produto != null && !catalogoEntry.Produtos.Any(x => x.Id == produto.Id))
                    {
                        catalogoEntry.Produtos.Add(produto);
                    }

                    if (arquivo != null && !catalogoEntry.Arquivos.Any(x => x.Id == arquivo.Id))
                    {
                        catalogoEntry.Arquivos.Add(arquivo);
                    }

                    return catalogoEntry;
                }, new { },
                splitOn: "NomeSubLinha,NomeProduto,NomeArquivo").Distinct().ToList();
            }

            return retorno;
        }

        public bool SalvarArquivoCatalogo(Arquivo arquivo)
        {
            try
            {
                if (arquivo.IdPai != null)
                {
                    DeletarArquivosExistentes(arquivo.IdPai.Value);

                    arquivo.NomeArquivo = Path.GetFileNameWithoutExtension(arquivo.NomeArquivo);
                    arquivoService.InsertArquivo(arquivo);
                }
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        private void DeletarArquivosExistentes(Guid idPai)
        {
            Arquivo arquivoExistente;

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                arquivoExistente = conexao.Query<Arquivo>(
                $@"
                    SELECT Id
                          ,NomeArquivo
                          ,Caminho
                          ,IdPai
                    FROM Arquivo
                    WHERE IdPai = @idPai 
                          AND IdTipoArquivo in (SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'catalogoLinhaCompleta' 
                                                                              OR CodigoTipoArquivo = 'catalogoProduto')
                "
                , new { idPai }).FirstOrDefault();
            }

            if(arquivoExistente != null)
            {
                arquivoService.DelArquivo(arquivoExistente.Id);
            }
        }
    }
}
