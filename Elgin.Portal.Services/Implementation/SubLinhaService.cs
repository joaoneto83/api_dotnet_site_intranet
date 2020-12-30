using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.Transactions;

namespace Elgin.Portal.Services.Implementation
{
    public class SubLinhaService : AbstractService<SubLinhaService>
    {
        private ArquivoService arquivoService;
        private ImagemService imagemService;

        public SubLinhaService(string connectionString, FileSettings fileSettings) : base(connectionString)
        {
            arquivoService = new ArquivoService(connectionString, fileSettings);
            imagemService = new ImagemService(connectionString, fileSettings);
        }

        public SubLinha ProcurarPorCodigo(string codigoSublinha, int idioma)
        {
            var retorno = new SubLinha();
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<SubLinha>(
                @"
                    SELECT  
                        Sublinha.Id
		                ,Sublinha.CodigoSubLinha
                        ,CASE WHEN SubLinhaIdioma.NomeSubLinha IS NULL OR SubLinhaIdioma.NomeSubLinha = '' THEN
		                    SubLinha.NomeSubLinha
	                    ELSE
		                    SubLinhaIdioma.NomeSubLinha
	                    END as NomeSubLinha
                        ,Sublinha.IdLinha
                        ,Sublinha.Ativo
                        ,Sublinha.Ordem
                        ,Sublinha.MostraAparelhoIdeal
                        ,Sublinha.Mostralink
                        ,Sublinha.MostraRota
                        ,Sublinha.TextoUrl
                        ,CASE WHEN SubLinhaIdioma.TextoBotao IS NULL OR SubLinhaIdioma.TextoBotao = '' THEN
		                    SubLinha.TextoBotao
	                    ELSE
		                    SubLinhaIdioma.TextoBotao
                        END as TextoBotao
                        ,CASE WHEN SubLinhaIdioma.TextoInformativo IS NULL OR SubLinhaIdioma.TextoInformativo = '' THEN
		                    SubLinha.TextoInformativo
	                    ELSE
		                    SubLinhaIdioma.TextoInformativo
                        END as TextoInformativo
                        ,Arquivo.Caminho AS CaminhoImagem
                    FROM SubLinha
                    LEFT JOIN Arquivo
                    ON Arquivo.Id = Sublinha.IdArquivo
                    LEFT JOIN SubLinhaIdioma
                    ON SubLinha.Id = SubLinhaIdioma.IdSublinha AND SubLinhaIdioma.CodigoIdioma = @idioma
                    WHERE Sublinha.Ativo = 1
                    AND CodigoSublinha = @codigoSublinha
                ", new { codigoSublinha, idioma }).FirstOrDefault();
            }

            retorno.Banner = arquivoService.GetArquivoPorIdPaiTipo(retorno.Id, "imgSublinhaAltaRes");
            retorno.Banner2 = arquivoService.GetArquivoPorIdPaiTipo(retorno.Id, "imgSublinhaBaixaRes");

            return retorno;
        }

        public List<SubLinha> ListarSubLinhas(Guid idLinha)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<SubLinha>(
                @"
                    SELECT  
                        Sublinha.Id
		                ,Sublinha.CodigoSubLinha
                        ,Sublinha.NomeSubLinha
                        ,Sublinha.IdLinha
                        ,Sublinha.Ativo
                        ,Sublinha.Ordem
                        ,Sublinha.MostraAparelhoIdeal
                        ,Arquivo.Caminho AS CaminhoImagem
                    FROM SubLinha
                    LEFT JOIN Arquivo
                    ON Arquivo.Id = Sublinha.IdArquivo
                        WHERE Sublinha.Ativo = 1
                        AND IdLinha = @idLinha
                    ORDER BY NomeSubLinha
                ", new { idLinha }).ToList();
            }
        }

        public List<SubLinha> ListarSubLinhasCodigoLinha(string codigoLinha, int? idioma)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<SubLinha>(
                @"
                    SELECT  
                        Sublinha.Id
		                ,Sublinha.CodigoSubLinha
                        ,CASE WHEN SubLinhaIdioma.NomeSubLinha IS NULL OR SubLinhaIdioma.NomeSubLinha = '' OR @idioma IS NULL THEN
		                    SubLinha.NomeSubLinha
	                    ELSE
		                    SubLinhaIdioma.NomeSubLinha
	                    END as NomeSubLinha
                        ,Sublinha.IdLinha
                        ,Sublinha.Ativo
                        ,Sublinha.Ordem
                        ,Sublinha.MostraAparelhoIdeal
                        ,Arquivo.Caminho AS CaminhoImagem
                    FROM SubLinha
                    LEFT JOIN Arquivo
                    ON Arquivo.Id = Sublinha.IdArquivo
                    LEFT JOIN SubLinhaIdioma
                    ON SubLinha.Id = SubLinhaIdioma.IdSublinha AND SubLinhaIdioma.CodigoIdioma = @idioma
                    WHERE Sublinha.Ativo = 1
                    AND IdLinha = (SELECT Id FROM Linha WHERE CodigoLinha = @codigoLinha)
                    ORDER BY Sublinha.Ordem, NomeSubLinha
                ", new { codigoLinha, idioma }).ToList();
            }
        }

        public List<SubLinha> ListarSubLinhasProdutos(string codigo)
        {
            var retorno = new List<SubLinha>();

            var sql = @"
                  SELECT  
                        SubLinha.Id
	                    ,SubLinha.CodigoSubLinha
                        ,SubLinha.NomeSubLinha
                        ,SubLinha.IdLinha
                        ,SubLinha.Ativo
                        ,Sublinha.Ordem
                        ,Sublinha.MostraAparelhoIdeal
	                    ,Produto.NomeProduto
	                    ,Produto.Id
                    FROM SubLinha
                    INNER JOIN Linha
                    ON Linha.Id = Sublinha.IdLinha
                    LEFT JOIN Produto
                    ON Produto.IdSublinha = SubLinha.Id
                    WHERE Linha.CodigoLinha = @codigo
                        AND SubLinha.Ativo = 1
                    ORDER BY NomeSubLinha
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, SubLinha>();

                retorno = conexao.Query<SubLinha, Produto, SubLinha>(sql,
                (sublinha, produto) =>
                {
                    SubLinha _sublinha;

                    if (!dictionary.TryGetValue(sublinha.Id, out _sublinha))
                    {
                        _sublinha = sublinha;
                        _sublinha.Produtos = new List<Produto>();
                        dictionary.Add(_sublinha.Id, _sublinha);
                    }

                    if (produto != null)
                        _sublinha.Produtos.Add(produto);

                    return _sublinha;
                }, new { codigo },
                splitOn: "NomeProduto").Distinct().ToList();
            }

            return retorno;
        }

        public SubLinha PreencheSubLinha(Guid id)
        {
            var retorno = new SubLinha();

            retorno = RetornaSubLinhaPorId(id);

            retorno.Banner = arquivoService.GetArquivoPorIdPaiTipo(retorno.Id, "imgSublinhaAltaRes");

            retorno.Banner2 = arquivoService.GetArquivoPorIdPaiTipo(retorno.Id, "imgSublinhaBaixaRes");

            retorno.Classificacoes = RetornaClassificacoesPorSublinha(id);

            retorno.Especificacoes = RetornaEspecificacoesPorSublinha(id);

            return retorno;
        }

        private SubLinha RetornaSubLinhaPorId(Guid id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<SubLinha>(
                @"
                    SELECT SubLinha.Id
		                ,CodigoSubLinha
                        ,NomeSubLinha
                        ,IdLinha
                        ,SubLinha.Ativo
                        ,SubLinha.Ordem
                        ,MostraAparelhoIdeal
                        ,MostraRota
                        ,MostraLink
                        ,TextoUrl
                        ,TextoBotao
                        ,TextoInformativo
                        ,PossuiFiltroPilha
                        ,Arquivo.Caminho AS CaminhoImagem
                        ,Arquivo.Id AS IdArquivo
                        ,Arquivo.NomeArquivo AS NomeArquivo
                    FROM SubLinha
                    LEFT JOIN Arquivo
                        ON Arquivo.Id = Sublinha.IdArquivo
                    WHERE SubLinha.Id = @id
                ", new { id }).FirstOrDefault();
            }
        }

        private List<Classificacao> RetornaClassificacoesPorSublinha(Guid idSublinha)
        {
            var retorno = new List<Classificacao>();

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Classificacao>(
                @"
                    SELECT Id
                        ,NomeClassificacao
                        ,IdSubLinha
                        ,Ativo
                        ,IdClassificacaoSuperior
                        ,CaminhoImagem
                        ,OrdemImagem
                        ,Comparativo
                    FROM Classificacao
                    WHERE IdSubLinha = @idSublinha
                ", new { idSublinha }).ToList();
            }

            foreach (var item in retorno)
            {
                item.Filhos.AddRange(retorno.Where(x => x.IdClassificacaoSuperior == item.Id).ToList());
            }

            return retorno.Where(x => x.IdClassificacaoSuperior == null).ToList();
        }
        private List<EspecificacaoTecnica> RetornaEspecificacoesPorSublinha(Guid idSublinha)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<EspecificacaoTecnica>(
                @"
                    SELECT Id
                        ,CodigoEspecificacao
                        ,NomeEspecificacao
                        ,IdSubLinha
                        ,Ativo
                        ,Comparativo
                    FROM EspecificacaoTecnica
                    WHERE IdSubLinha = @idSublinha
                ", new { idSublinha }).ToList();
            }
        }

        public SubLinha Salvar(SubLinha model)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    if (model.MostraLink)
                    {
                        if (!model.TextoUrl.Contains("http://") && !model.TextoUrl.Contains("https://"))
                        {
                            model.TextoUrl = "http://" + model.TextoUrl;
                        }
                    }

                    model.Arquivo = arquivoService.AddArquivo(model.Arquivo);

                    model.IdArquivo = model.Arquivo.Id;

                    if (model.Id != Guid.Empty)
                    {
                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                             UPDATE [SubLinha]
                                SET [Id] = @Id
                                    ,[CodigoSubLinha] = @CodigoSubLinha
                                    ,[NomeSubLinha] = @NomeSubLinha
                                    ,[IdLinha] = @IdLinha
                                    ,[Ativo] = @Ativo
                                    ,[Ordem] = @Ordem
                                    ,[MostraAparelhoIdeal] = @MostraAparelhoIdeal
                                    ,[MostraRota] = @MostraRota
                                    ,[MostraLink] = @MostraLink
                                    ,[TextoUrl] = @TextoUrl
                                    ,[TextoBotao] = @TextoBotao
                                    ,[TextoInformativo] = @TextoInformativo
                                    ,[PossuiFiltroPilha] = @PossuiFiltroPilha
                                    ,[IdArquivo] = @IdArquivo
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
                             INSERT INTO [SubLinha]
                                        ([Id]
                                        ,[CodigoSubLinha]
                                        ,[NomeSubLinha]
                                        ,[IdLinha]
                                        ,[Ativo]
                                        ,[IdArquivo]
                                        ,[Ordem]
                                        ,[MostraAparelhoIdeal]
                                        ,[MostraLink]
                                        ,[MostraRota]
                                        ,[TextoUrl]
                                        ,[TextoBotao]
                                        ,[TextoInformativo]
                                        ,[PossuiFiltroPilha])
                                  VALUES
                                        (@Id
                                        ,@CodigoSubLinha
                                        ,@NomeSubLinha
                                        ,@IdLinha
                                        ,@Ativo
                                        ,@IdArquivo
                                        ,@Ordem
                                        ,@MostraAparelhoIdeal
                                        ,@MostraLink
                                        ,@MostraRota
                                        ,@TextoUrl
                                        ,@TextoBotao
                                        ,@TextoInformativo
                                        ,@PossuiFiltroPilha)
                        ", model);
                        }
                    }

                    model = SalvarBanners(model);

                    RemoverEspecificacoes(model);
                    model = SalvarEspecificacoes(model);

                    //RemoverClassificacoes(model);
                    model = SalvarClassificacoes(model);

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return model;
        }

        private SubLinha SalvarBanners(SubLinha model)
        {
            if (!string.IsNullOrEmpty(model.Banner.Caminho))
            {
                model.Banner.IdPai = model.Id;

                if (model.Banner.Id == Guid.Empty)
                {
                    var bannerOld = arquivoService.GetArquivoPorIdPaiTipo(model.Id, "imgSublinhaAltaRes");
                    if (bannerOld != null)
                        arquivoService.DelArquivo(bannerOld.Id);
                }

                arquivoService.AddArquivo(model.Banner);
            }

            if (!string.IsNullOrEmpty(model.Banner2.Caminho))
            {
                model.Banner2.IdPai = model.Id;

                if (model.Banner2.Id == Guid.Empty)
                {
                    var banner2Old = arquivoService.GetArquivoPorIdPaiTipo(model.Id, "imgSublinhaBaixaRes");

                    if (banner2Old != null)
                        arquivoService.DelArquivo(banner2Old.Id);
                }

                arquivoService.AddArquivo(model.Banner2);
            }

            return model;
        }

        private SubLinha SalvarClassificacoes(SubLinha model)
        {
            foreach (var classificacao in model.Classificacoes)
            {
                RemoverClassificacoesFilhos(classificacao, model.Id);

                if (classificacao.Id != Guid.Empty)
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             UPDATE [Classificacao]
                                SET [NomeClassificacao] = @NomeClassificacao
                                    ,[IdSubLinha] = @IdSubLinha
                                    ,[Ativo] = @Ativo
                                    ,[Ordem] = @Ordem
                                    ,[Expansivo] = @Expansivo
                                WHERE Id = @Id
                        ", new
                        {
                            classificacao.Id,
                            classificacao.Ativo,
                            classificacao.NomeClassificacao,
                            classificacao.Ordem,
                            classificacao.Expansivo,
                            IdSubLinha = model.Id
                        });
                    }
                }

                else
                {
                    classificacao.Id = Guid.NewGuid();

                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             INSERT INTO [Classificacao]
                                        ([Id]
                                        ,[NomeClassificacao]
                                        ,[IdSubLinha]
                                        ,[Ativo]
                                        ,[Ordem]
                                        ,[Expansivo])
                                  VALUES
                                        (@Id
                                        ,@NomeClassificacao
                                        ,@IdSubLinha
                                        ,@Ativo
                                        ,@Ordem
                                        ,@Expansivo)
                        ", new
                        {
                            classificacao.Id,
                            classificacao.Ativo,
                            classificacao.NomeClassificacao,
                            classificacao.Ordem,
                            classificacao.Expansivo,
                            IdSubLinha = model.Id
                        });
                    }
                }

                foreach (var filho in classificacao.Filhos)
                {
                    if (filho.Id != Guid.Empty)
                    {
                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                             UPDATE [Classificacao]
                                SET [NomeClassificacao] = @NomeClassificacao
                                    ,[IdSubLinha] = @IdSubLinha
                                    ,[Ativo] = @Ativo
                                    ,[IdClassificacaoSuperior] = @IdClassificacaoSuperior
                                    ,[Comparativo] = @Comparativo
                                    ,[Ordem] = @Ordem
                                    ,[Expansivo] = @Expansivo
                                WHERE Id = @Id
                        ", new
                            {
                                filho.Id,
                                filho.Comparativo,
                                filho.Ativo,
                                filho.NomeClassificacao,
                                filho.Ordem,
                                filho.Expansivo,
                                IdSubLinha = model.Id,
                                IdClassificacaoSuperior = classificacao.Id
                            });
                        }
                    }

                    else
                    {
                        filho.Id = Guid.NewGuid();

                        using (SqlConnection conexao = new SqlConnection(ConnectionString))
                        {
                            conexao.Execute(
                            @"
                             INSERT INTO [Classificacao]
                                        ([Id]
                                        ,[NomeClassificacao]
                                        ,[IdSubLinha]
                                        ,[Ativo]
                                        ,[IdClassificacaoSuperior]
                                        ,[Comparativo])
                                  VALUES
                                        (@Id
                                        ,@NomeClassificacao
                                        ,@IdSubLinha
                                        ,@Ativo
                                        ,@IdClassificacaoSuperior
                                        ,@Comparativo)
                        ", new
                            {
                                filho.Id,
                                filho.Comparativo,
                                filho.Ativo,
                                filho.NomeClassificacao,
                                IdSubLinha = model.Id,
                                IdClassificacaoSuperior = classificacao.Id
                            });
                        }
                    }

                }
            }

            return model;
        }

        private SubLinha SalvarEspecificacoes(SubLinha model)
        {
            foreach (var espec in model.Especificacoes)
            {
                if (espec.Id != Guid.Empty)
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             UPDATE [EspecificacaoTecnica]
                                SET [Id] = @Id
                                    ,[CodigoEspecificacao] = @CodigoEspecificacao
                                    ,[NomeEspecificacao] = @NomeEspecificacao
                                    ,[IdSubLinha] = @IdSubLinha
                                    ,[Ativo] = @Ativo
                                    ,[Comparativo] = @Comparativo
                                WHERE Id = @Id
                        ", new
                        {
                            espec.Id,
                            espec.CodigoEspecificacao,
                            espec.NomeEspecificacao,
                            IdSubLinha = model.Id,
                            espec.Ativo,
                            espec.Comparativo,
                        });
                    }
                }

                else
                {
                    espec.Id = Guid.NewGuid();

                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             INSERT INTO [EspecificacaoTecnica]
                                        ([Id]
                                        ,[CodigoEspecificacao]
                                        ,[NomeEspecificacao]
                                        ,[IdSubLinha]
                                        ,[Ativo]
                                        ,[Comparativo])
                                  VALUES
                                        (@Id
                                        ,@CodigoEspecificacao
                                        ,@NomeEspecificacao
                                        ,@IdSubLinha
                                        ,@Ativo
                                        ,@Comparativo)
                        ", new
                        {
                            Id = espec.Id,
                            CodigoEspecificacao = espec.CodigoEspecificacao,
                            NomeEspecificacao = espec.NomeEspecificacao,
                            IdSubLinha = model.Id,
                            Ativo = espec.Ativo,
                            Comparativo = espec.Comparativo,
                        });
                    }
                }
            }

            return model;
        }

        private void RemoverEspecificacoes(SubLinha model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [EspecificacaoTecnica]
                         WHERE Id NOT IN @Ids AND IdSublinha = @IdSublinha
                ", new
                {
                    Ids = model.Especificacoes.Select(x => x.Id).ToList(),
                    IdSublinha = model.Id
                });
            }
        }

        private void RemoverClassificacoesFilhos(Classificacao model, Guid idSublinha)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [Classificacao]
                         WHERE Id NOT IN @Ids 
                               AND IdSublinha = @IdSublinha 
                               AND IdClassificacaoSuperior = @classificacaoPai
                ", new
                {
                    Ids = model.Filhos.Select(x => x.Id).ToList(),
                    IdSublinha = idSublinha,
                    classificacaoPai = model.Id
                });
            }
        }
    }
}
