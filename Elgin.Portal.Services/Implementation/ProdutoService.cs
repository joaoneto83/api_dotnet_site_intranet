using Dapper;
using Elgin.Portal.Services.Implementation.Shared;
using Elgin.Portal.Services.Model;
using Elgin.Portal.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;


namespace Elgin.Portal.Services.Implementation
{
    public class ProdutoService : AbstractService<ProdutoService>
    {
        private DominioService dominioService;
        private ArquivoService arquivoService;
        private EspecificacaoTecnicaService especService;

        public ProdutoService(string connectionString, FileSettings fileSettings) : base(connectionString)
        {
            dominioService = new DominioService(connectionString);
            arquivoService = new ArquivoService(connectionString, fileSettings);
            especService = new EspecificacaoTecnicaService(connectionString);
        }

        public CalculoSolarResultado RetornaCalculoSolar(CalculoSolar model)
        {
            var retorno = new CalculoSolarResultado();
            retorno.calculo = model;

            var estado = dominioService.EstadoPorUF(model.estado);
            var cidade = dominioService.CidadePorId(model.idCidade);
            var variaveis = getSolarVariaveis();
            var listaPotenciaPlacas = getSolarPotenciaPlacas();
            var listaIntegradores = dominioService.getSolarIntegradores(model.estado, model.idCidade);

            decimal consumo = 0;

            if (model.mediaMensal != null)
            {
                consumo = model.mediaMensal.Value;
            }
            else
            {
                consumo = model.faturaMensal / variaveis.MediaConsumo;
            }

            var kit = ((((consumo / 30) / cidade.SolarMediaAnual) * (1 + variaveis.PerdaSistema)) * (1 + variaveis.PerdaPlaca));

            retorno.kitNecessario = kit;

            var kitEscolhido = listaPotenciaPlacas.Where(x => x.Potencia >= kit).Min(x => x.Potencia);
            var kitEscolhidoPaineis = listaPotenciaPlacas.FirstOrDefault(x => x.Potencia == kitEscolhido).Placas;

            retorno.kit1 = kitEscolhido;

            decimal kitAdicional = 0;
            int kitAdicionalPaineis = 0;

            var diferenca = kitEscolhido - kit;
            if (diferenca > (decimal)0.6)
            {
                kitEscolhido = listaPotenciaPlacas.Where(x => x.Potencia <= kit).Max(x => x.Potencia);
                kitEscolhidoPaineis = listaPotenciaPlacas.FirstOrDefault(x => x.Potencia == kitEscolhido).Placas;
                diferenca = kit - kitEscolhido;

                kitAdicional = listaPotenciaPlacas.Where(x => x.Potencia >= diferenca).Min(x => x.Potencia);
                kitAdicionalPaineis = listaPotenciaPlacas.FirstOrDefault(x => x.Potencia == kitAdicional).Placas;

                retorno.kit1 = kitEscolhido;
                retorno.kit2 = kitAdicional;
            }

            var espaco = decimal.Round((kitEscolhidoPaineis + kitAdicionalPaineis) * (decimal)1.940, 2);

            retorno.estimativaArea = espaco;

            var geracao = (((kitEscolhido + kitAdicional) * cidade.SolarMediaAnual * 30) / (1 + variaveis.PerdaSistema)) / (1 + variaveis.PerdaPlaca);

            retorno.economiaFinanceira = decimal.Round((geracao * estado.TarifaSolar) * 12, 2);

            retorno.economiaEnergia = decimal.Round(((geracao * estado.TarifaSolar) / model.faturaMensal) * 100, 2);

            if (retorno.economiaEnergia > 100)
            {
                retorno.economiaEnergia = 99;
            }

            if (cidade.SolarMediaAnual <= (decimal)4.5)
            {
                retorno.condicaoSolar = "Bom";
            }
            else if (cidade.SolarMediaAnual > (decimal)4.5 && cidade.SolarMediaAnual <= (decimal)4.8)
            {
                retorno.condicaoSolar = "Ótimo";
            }
            else
            {
                retorno.condicaoSolar = "Excelente";
            }

            retorno.qtdIntegradores = listaIntegradores.Count();

            return retorno;
        }

        public List<Produto> RetornaProdutosPorSubLinha(Guid id, int? idioma)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Produto>(
                 @"
                   SELECT 
	                    Produto.Id, 
	                    CASE WHEN ProdutoIdioma.NomeProduto IS NULL OR ProdutoIdioma.NomeProduto = '' OR @idioma IS NULL THEN
		                   Produto.NomeProduto
	                    ELSE
		                   ProdutoIdioma.NomeProduto
                        END AS NomeProduto
                    FROM Produto
                    LEFT JOIN ProdutoIdioma
                    ON Produto.Id = ProdutoIdioma.IdProduto AND ProdutoIdioma.CodigoIdioma = @idioma
                    WHERE Produto.IdSublinha = @id
                ", new { id, idioma }).ToList();
            }
        }

        public List<Produto> RetornaProdutosPorLinha(Guid id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Produto>(
                 @"
                   SELECT 
	                    Produto.Id, 
	                    Produto.NomeProduto,
                        Produto.ativo
                    FROM Produto
                    JOIN Sublinha
                    ON Produto.IdSublinha = Sublinha.Id
                    WHERE IdLinha = @id
                ", new { id }).ToList();
            }
        }

        public void SalvarCalculoSolar(CalculoSolarResultado model)
        {
            var estado = dominioService.EstadoPorUF(model.calculo.estado);
            var cidade = dominioService.CidadePorId(model.calculo.idCidade);
            var onde = model.calculo.onde == "C" ? "Casa" : "Empresa";

            var newModel = new SolarSimulacoes
            {
                Nome = model.nome,
                Email = model.email,
                Cidade = cidade.Descricao,
                Estado = estado.Descricao,
                IdCidade = model.calculo.idCidade,
                UF = model.calculo.estado,
                Onde = onde,
                FormatoFatura = "",
                FaturaMensal = model.calculo.faturaMensal.ToString(),
                MediaMensal = model.calculo.mediaMensal.ToString(),
                CondicaoSolar = model.condicaoSolar,
                EcominaFinanceira = model.economiaFinanceira.ToString(),
                Economia = model.economiaEnergia.ToString(),
                EstimativaArea = model.estimativaArea.ToString(),
                Kit1 = model.kit1.ToString(),
                Kit2 = model.kit2.ToString(),
                KitNecessario = model.kitNecessario.ToString(),
                KitTotal = model.kitTotal.ToString()
            };

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        INSERT INTO [SolarSimulacoes]
                                   ([Nome]
                                   ,[Email]
                                   ,[Estado]
                                   ,[Cidade]
                                   ,[IdCidade]
                                   ,[UF]
                                   ,[Onde]
                                   ,[FormatoFatura]
                                   ,[FaturaMensal]
                                   ,[MediaMensal]
                                   ,[CondicaoSolar]
                                   ,[EstimativaArea]
                                   ,[Economia]
                                   ,[EcominaFinanceira]
                                   ,[Kit1]
                                   ,[Kit2]
                                   ,[KitTotal]
                                   ,[KitNecessario]
                                   )
                             VALUES
                                   (@Nome
                                   ,@Email
                                   ,@Estado
                                   ,@Cidade
                                   ,@IdCidade
                                   ,@UF
                                   ,@Onde
                                   ,@FormatoFatura
                                   ,@FaturaMensal
                                   ,@MediaMensal
                                   ,@CondicaoSolar
                                   ,@EstimativaArea
                                   ,@Economia
                                   ,@EcominaFinanceira
                                   ,@Kit1
                                   ,@Kit2
                                   ,@KitTotal
                                   ,@KitNecessario)
                        ", newModel);
            }
        }

        public bool Salvar(Produto model)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                            UPDATE [dbo].[Produto]
                               SET 
                                   [CodigoProduto] = @CodigoProduto
                                  ,[CodigoLegado] = @CodigoLegado
                                  ,[NomeProduto] = @NomeProduto
                                  ,[IdSubLinha] = @IdSubLinha
                                  ,[Ativo] = @Ativo
                                  ,[Preco] = @Preco
                            WHERE Id = @Id
                        ", model);
                    }

                    RemoverCaracteristicas(model);
                    AtualizarCaracteristicas(model);
                    SalvarCaracteristicas(model);

                    RemoverModelos(model);
                    AtualizarModelos(model);
                    SalvarModelos(model);

                    RemoverEspecificacaoTecnica(model);
                    SalvarEspecificacaoTecnica(model);

                    RemoverClassificacoes(model);
                    SalvarClassificacoes(model);

                    RemoverSecoesProduto(model);
                    AtualizarSecoesProduto(model);
                    AtualizarArquivosSecoesProduto(model);

                    RemoverSecaoProdutoIcone(model);
                    SalvarSecaoProdutoIcone(model);
                    AtualizarSecaoProdutoIcone(model);

                    RemoverPalavrasChave(model);
                    AtualizarPalavrasChave(model);
                    SalvarPalavrasChave(model);

                    transaction.Complete();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void SalvarSecaoProdutoIcone(Produto model)
        {
            var icones = new List<SecaoProdutoIcone>();
            foreach (var item in model.SecoesProduto)
            {
                //if (item.CodigoSecao == "secao4")
                //{
                icones.AddRange(item.Icones.Where(x => x.Id == Guid.Empty));
                //}
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        INSERT INTO [dbo].[SecaoProdutoIcone]
                                    ([DescricaoIcone]
                                    ,[SubDescricaoIcone]
                                    ,[IdSecaoProduto]
                                    ,[IdIcone]
                                    ,[Ordem])
                                VALUES
                                    (@DescricaoIcone
                                    ,@SubDescricaoIcone
                                    ,@IdSecaoProduto
                                    ,@IdIcone
                                    ,@Ordem)

                ", icones);
            }
        }

        private void AtualizarSecaoProdutoIcone(Produto model)
        {
            var icones = new List<SecaoProdutoIcone>();
            foreach (var item in model.SecoesProduto)
            {
                //if (item.CodigoSecao == "secao4")
                //{
                icones.AddRange(item.Icones.Where(x => x.Id != Guid.Empty));
                //}
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE [dbo].[SecaoProdutoIcone]
                       SET 
                           [DescricaoIcone] = @DescricaoIcone
                          ,[SubDescricaoIcone] = @SubDescricaoIcone
                          ,[IdIcone] = @IdIcone
                          ,[Ordem] = @Ordem
                     WHERE Id = @Id

                ", icones);
            }
        }

        private void RemoverSecaoProdutoIcone(Produto model)
        {
            foreach (var item in model.SecoesProduto)
            {
                if (item.Icones == null)
                {
                    continue;
                }

                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Execute(
                    @"
                        DELETE [SecaoProdutoIcone]
                         WHERE Id NOT IN @Ids AND IdSecaoProduto = @IdSecaoProduto
                ", new
                    {
                        Ids = item.Icones.Select(x => x.Id),
                        IdSecaoProduto = item.Id
                    });
                }
            }
        }

        private void RemoverPalavrasChave(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [PalavraChave]
                         WHERE Id NOT IN @Ids AND IdProduto = @IdProduto
                ", new
                {
                    Ids = model.PalavrasChave.Select(x => x.Id).ToList(),
                    IdProduto = model.Id
                });
            }
        }

        private void AtualizarPalavrasChave(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        UPDATE [PalavraChave]
                           SET [Valor] = @Valor
                         WHERE Id = @Id
                ", model.PalavrasChave.Where(x => x.Id != Guid.Empty));
            }
        }

        private void SalvarPalavrasChave(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        INSERT INTO [dbo].[PalavraChave]
                                   ([Valor]
                                   ,[IdProduto])
                             VALUES
                                   (@Valor
                                   ,@IdProduto)
                ", model.PalavrasChave.Where(x => x.Id == Guid.Empty));
            }
        }

        private void RemoverAparelhoIdeal(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [ProdutoAparelhoIdeal]
                         WHERE IdProduto = @IdProduto
                ", new
                {
                    IdProduto = model.Id
                });
            }
        }

        private void AtualizarArquivosSecoesProduto(Produto model)
        {
            var arquivos = new List<Arquivo>();

            foreach (var item in model.SecoesProduto)
            {
                arquivos.AddRange(item.Arquivos);
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE [Arquivo]
                       SET [NomeArquivo] = @NomeArquivo
                          ,[Ordem] = @Ordem
                          ,[Ativo] = @Ativo
                          ,[Linque] = @Linque

                         WHERE Id = @Id

                ", arquivos);
            }
        }

        private void AtualizarSecoesProduto(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE [SecaoProduto]
                       SET [Texto1] = @Texto1
                          ,[Texto2] = @Texto2
                          ,[Texto3] = @Texto3
                          ,[CodigoVideo] = @CodigoVideo
                          ,[CodigoVideo2] = @CodigoVideo2
                          ,[CodigoVideo3] = @CodigoVideo3
                          ,[CodigoVideo4] = @CodigoVideo4
                          ,[AparelhoIdeal] = @AparelhoIdeal
                          ,[Ordem] = @Ordem
                          ,[Ativo] = @Ativo
                         WHERE Id = @Id

                ", model.SecoesProduto);
            }
        }

        private void RemoverSecoesProduto(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [SecaoProduto]
                         WHERE Id NOT IN @Ids AND IdProduto = @IdProduto
                ", new
                {
                    Ids = model.SecoesProduto.Select(x => x.Id).ToList(),
                    IdProduto = model.Id
                });
            }
        }

        private void RemoverSecaoProduto(SecaoProduto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [SecaoProduto]
                         WHERE  Id = @Id
                ", new
                {
                    model.Id
                });
            }
        }

        private void SalvarEspecificacaoTecnica(Produto model)
        {
            model.EspecificacoesTecnicas.ForEach(x => x.IdProduto = model.Id);

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        INSERT INTO [dbo].[ProdutoEspecificacaoTecnica]
                                   ([IdProduto]
                                   ,[IdEspecificacaoTecnica]
                                   ,[Valor])
                             VALUES
                                   (@IdProduto
                                   ,@Id
                                   ,@Valor)

                ", model.EspecificacoesTecnicas);
            }
        }

        private void RemoverEspecificacaoTecnica(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                       DELETE FROM ProdutoEspecificacaoTecnica WHERE IdProduto = @Id

                ", model);
            }
        }

        private void SalvarClassificacoes(Produto model)
        {
            var filhos = new List<Classificacao>();

            foreach (var c in model.Classificacoes)
            {
                filhos.AddRange(c.Filhos.Where(x => x.Selecionado));
            }

            filhos.ForEach(x => x.IdProduto = model.Id);


            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        INSERT INTO [dbo].[ProdutoClassificacao]
                                   ([IdProduto]
                                   ,[IdClassificacao])
                             VALUES
                                   (@IdProduto
                                   ,@Id)
                ", filhos);
            }

        }

        private void RemoverClassificacoes(Produto model)
        {

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                       DELETE FROM ProdutoClassificacao WHERE IdProduto = @Id

                ", model);
            }
        }

        private void RemoverCaracteristicas(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [Caracteristica]
                         WHERE Id NOT IN @Ids AND IdProduto = @IdProduto
                ", new
                {
                    Ids = model.Caracteristicas.Select(x => x.Id).ToList(),
                    IdProduto = model.Id
                });
            }
        }

        private void AtualizarCaracteristicas(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        UPDATE [Caracteristica]
                           SET [DescricaoCaracteristica] = @DescricaoCaracteristica
                              ,[CaracteristicaPrincipal] = @CaracteristicaPrincipal
                              ,[IdIcone] = @IdIcone
                              ,[Ordem] = @Ordem
                         WHERE Id = @Id
                ", model.Caracteristicas.Where(x => x.Id != Guid.Empty));
            }
        }

        private void SalvarCaracteristicas(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        INSERT INTO [Caracteristica]
                                   ([DescricaoCaracteristica]
                                   ,[IdProduto]
                                   ,[CaracteristicaPrincipal]
                                   ,[IdIcone]
                                   ,[Ordem])
                             VALUES
                                   (@DescricaoCaracteristica
                                   ,@IdProduto
                                   ,@CaracteristicaPrincipal
                                   ,@IdIcone
                                   ,@Ordem)
                ", model.Caracteristicas.Where(x => x.Id == Guid.Empty));
            }
        }

        private void RemoverModelos(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [Modelo]
                         WHERE Id NOT IN @Ids AND IdProduto = @IdProduto
                ", new
                {
                    Ids = model.Modelos.Select(x => x.Id).ToList(),
                    IdProduto = model.Id
                });
            }
        }

        private void AtualizarModelos(Produto model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        UPDATE [Modelo]
                           SET [CodigoModelo] = @CodigoModelo
                              ,[NomeModelo] = @NomeModelo
                              ,[AparelhoIdealDe] = @AparelhoIdealDe
                              ,[AparelhoIdealAte] = @AparelhoIdealAte
                              ,[Ativo] = @Ativo
                         WHERE Id = @Id
                ", model.Modelos.Where(x => x.Id != Guid.Empty));
            }
        }

        private void SalvarModelos(Produto model)
        {
            model.Modelos.ForEach(x => x.IdProduto = model.Id);

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    INSERT INTO [dbo].[Modelo]
                                ([CodigoModelo]
                                ,[NomeModelo]
                                ,[IdProduto]
                                ,[AparelhoIdealDe]
                                ,[AparelhoIdealAte]
                                ,[Ativo])
                            VALUES
                                (@CodigoModelo
                                ,@NomeModelo
                                ,@IdProduto
                                ,@AparelhoIdealDe
                                ,@AparelhoIdealAte
                                ,@Ativo)
                ", model.Modelos.Where(x => x.Id == Guid.Empty));
            }
        }

        public Produto PreencheProduto(string produto, bool? ativo, int? idioma)
        {
            var objProduto = RetornaProduto(produto, idioma, ativo);

            objProduto.Arquivos = arquivoService.RetornaArquivosProduto(objProduto.Id, ativo, idioma);

            objProduto.Caracteristicas = RetornaCaracteristicasProduto(objProduto.Id, idioma);

            objProduto.Classificacoes = RestornaClassificacoesProduto(objProduto.IdSubLinha, objProduto.Id, true);

            objProduto.EspecificacoesTecnicas = especService.ListarEspecificacoesTecnicas(objProduto.IdSubLinha, objProduto.Id, true, idioma);

            objProduto.Modelos = RetornaModelosProduto(objProduto.Id, ativo);

            objProduto.SecoesProduto = RetornaSecoesProduto(objProduto.Id, ativo, idioma);

            objProduto.PalavrasChave = RetornaPalavrasChave(objProduto.Id);

            return objProduto;
        }

        public List<ResultadoInputBusca> PesquisaProdutos(string termoPesquisa)
        {
            List<ResultadoInputBusca> retorno;

            if (string.IsNullOrEmpty(termoPesquisa) == false)
            {
                termoPesquisa = "%" + termoPesquisa + "%";
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<ResultadoInputBusca>(
                $@"
                    SELECT DISTINCT TOP 3 p.[NomeProduto] AS Texto
                                  ,(
                                        SELECT TOP 1 a.Caminho FROM Arquivo a
                                        WHERE a.IdPai = p.ID AND a.Ordem = 1 AND a.Ativo = 1 AND
                                        A.IdTipoArquivo = (Select Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'imgCard')
                                   ) AS ImagemUrl
                                  ,CONCAT('/SuporteTecnico/', P.CodigoProduto) as Link
                    FROM Produto AS p
                    LEFT JOIN PalavraChave PC ON PC.IdProduto = P.Id
                    WHERE PC.Valor like @termoPesquisa 
                        OR P.nomeProduto like @termoPesquisa
                        OR @termoPesquisa = ''
                    ORDER BY P.NomeProduto
                "
                , new { termoPesquisa }).ToList();
            }

            foreach (var item in retorno)
            {
                item.Produto = true;
            }

            return retorno;

        }

        public List<ResultadoInputBusca> PesquisaProdutosDetalhes(string termoPesquisa)
        {
            List<ResultadoInputBusca> retorno;

            if (string.IsNullOrEmpty(termoPesquisa) == false)
            {
                termoPesquisa = "%" + termoPesquisa + "%";
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<ResultadoInputBusca>(
                $@"
                    SELECT DISTINCT P.[NomeProduto] AS Texto
                          ,(SELECT TOP 1 A.Caminho FROM Arquivo A
                              WHERE A.IdPai = P.ID AND A.Ordem = 1 AND A.Ativo = 1 AND
                              A.IdTipoArquivo = (Select Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'imgCard')
                           ) AS ImagemUrl
                           ,CONCAT('/Produtos/', L.CodigoLinha, '/', SL.CodigoSublinha, '/', P.CodigoProduto) as Link
                    FROM Produto AS P
                    LEFT JOIN PalavraChave PC ON PC.IdProduto = P.Id
                    Inner Join SubLinha SL on SL.Id = P.IdSubLinha
                    Inner Join Linha L on L.Id = SL.IdLinha
                    WHERE PC.Valor like @termoPesquisa 
                        OR P.nomeProduto like @termoPesquisa
                        OR @termoPesquisa = ''
                    ORDER BY P.NomeProduto
                "
                , new { termoPesquisa }).ToList();
            }

            return retorno;

        }

        private List<PalavraChave> RetornaPalavrasChave(Guid id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<PalavraChave>(
                @"
                    SELECT [Id]
                          ,[Valor]
                          ,[IdProduto]
                    FROM [PalavraChave]
                    WHERE IdProduto = @id
                "
                , new { id }).ToList();
            }
        }

        private List<Classificacao> RestornaClassificacoesProduto(Guid idSublinha, Guid id, bool? ativo)
        {
            var retorno = new List<Classificacao>();

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Classificacao>(
                @"
                    SELECT [Id]
                      ,[NomeClassificacao]
                      ,[IdSubLinha]
                      ,[Ativo]
                      ,[IdClassificacaoSuperior]
	                  , ProdutoClassificacao.IdProduto
	                  ,(CASE WHEN ProdutoClassificacao.IdProduto IS NULL THEN 0 ELSE 1 END) AS Selecionado
                  FROM [Classificacao]
                  LEFT JOIN ProdutoClassificacao
                  ON Classificacao.Id = ProdutoClassificacao.IdClassificacao  
                  AND ProdutoClassificacao.IdProduto = @id
                  WHERE Classificacao.IdSubLinha = @idSublinha AND (@ativo IS NULL OR Ativo = @ativo)
                ", new { id, idSublinha, ativo }).ToList();
            }

            foreach (var item in retorno)
            {
                item.Filhos.AddRange(retorno.Where(x => x.IdClassificacaoSuperior == item.Id).ToList());
            }

            var pilhas = RetornaPilhas(idSublinha, id, ativo);
            if (pilhas != null)
            {
                retorno.AddRange(pilhas);
            }

            return retorno.Where(x => x.IdClassificacaoSuperior == null).ToList();
        }

        private List<Classificacao> RetornaPilhas(Guid idSublinha, Guid id, bool? ativo)
        {
            var retorno = new List<Classificacao>();

            bool possuiPilha;

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                possuiPilha = conexao.Query<bool>(
                @"
                    SELECT PossuiFiltroPilha
                      FROM [SubLinha]
                      WHERE Id = @idSublinha
                ", new { idSublinha }).FirstOrDefault();
            }

            if (possuiPilha)
            {
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    retorno = conexao.Query<Classificacao>(
                    @"
                    SELECT [Id]
                      ,[NomeClassificacao]
                      ,[IdSubLinha]
                      ,[Ativo]
                      ,[IdClassificacaoSuperior]
	                  , ProdutoClassificacao.IdProduto
	                  ,(CASE WHEN ProdutoClassificacao.IdProduto IS NULL THEN 0 ELSE 1 END) AS Selecionado
                    FROM [Classificacao]
                      LEFT JOIN ProdutoClassificacao
                      ON Classificacao.Id = ProdutoClassificacao.IdClassificacao 
                      AND ProdutoClassificacao.IdProduto = @id
                    WHERE (NomeClassificacao = 'Pilhas' 
                            OR IdClassificacaoSuperior = (SELECT Id FROM Classificacao WHERE NomeClassificacao = 'Pilhas'))
                    AND (@ativo IS NULL OR Ativo = @ativo)
                    ORDER BY NomeClassificacao
                    ", new { id, idSublinha, ativo }).ToList();
                }

                foreach (var item in retorno)
                {
                    item.Filhos.AddRange(retorno.Where(x => x.IdClassificacaoSuperior == item.Id).ToList());
                }

                return retorno;
            }
            else
            {
                return null;
            }
        }

        private List<Caracteristica> RetornaCaracteristicasProduto(Guid id, int? idioma)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Caracteristica>(
                @"
                    SELECT Caracteristica.[Id]
                          ,CASE WHEN CI.DescricaoCaracteristica IS NULL OR CI.DescricaoCaracteristica = '' THEN
		                    Caracteristica.DescricaoCaracteristica
	                      ELSE
		                    CI.DescricaoCaracteristica
	                      END as DescricaoCaracteristica
                          ,Caracteristica.[IdProduto]
                          ,Caracteristica.[CaracteristicaPrincipal]
                          ,Caracteristica.[IdIcone]
                          ,Caracteristica.[Ordem]
                          ,Icone.[Caminho] AS urlIcone
                    FROM [dbo].[Caracteristica]
                    LEFT JOIN Arquivo Icone
                      ON Icone.Id = Caracteristica.IdIcone
                    LEFT JOIN CaracteristicaIdioma AS CI
                        ON Caracteristica.Id = CI.IdCaracteristica AND CI.CodigoIdioma = @idioma
                    WHERE Caracteristica.IdProduto = @id
                    ORDER BY Caracteristica.[Ordem]
                ", new { id, idioma }).ToList();
            }
        }

        private List<Modelo> RetornaModelosProduto(Guid id, bool? ativo)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Modelo>(
                    @"
                    SELECT [Id]
                          ,[NomeModelo]
                          ,[CodigoModelo]
                          ,[IdProduto]
                          ,[Ativo]
                      FROM [dbo].[Modelo]
                    WHERE IdProduto = @id AND (@ativo IS NULL OR Ativo = @ativo)
                    ORDER BY CodigoModelo
                ", new { id, ativo }).ToList();
            }
        }

        private List<EspecificacaoTecnica> RetornaEspecTecnicaProduto(Guid id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<EspecificacaoTecnica>(
                @"
                    SELECT [Id]
                          ,[CodigoEspecificacao]
                          ,[NomeEspecificacao]
                          ,[IdSubLinha]
                          ,[Ativo]
                     FROM [dbo].[EspecificacaoTecnica]
                     JOIN ProdutoEspecificacaoTecnica
                    ON ProdutoEspecificacaoTecnica.IdEspecificacaoTecnica = EspecificacaoTecnica.Id
                    WHERE ProdutoEspecificacaoTecnica.IdProduto = @id
                ", new { id }).ToList();
            }
        }

        public Produto RetornaProduto(string produto, int? idioma, bool? ativo)
        {
            var buscaPorId = Guid.TryParse(produto, out var idProduto);
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Produto>(
                @"
                    SELECT Produto.[Id]
                          ,Produto.[CodigoProduto]
                          ,Produto.[CodigoLegado]
                          ,CASE WHEN ProdutoIdioma.NomeProduto IS NULL OR ProdutoIdioma.NomeProduto = '' THEN
		                    Produto.NomeProduto
	                      ELSE
		                    ProdutoIdioma.NomeProduto
                          END as NomeProduto
                          ,Produto.[IdSubLinha]
	                      ,SubLinha.IdLinha
                          ,SubLinha.NomeSublinha
                          ,CASE WHEN SubLinhaIdioma.NomeSublinha IS NULL OR SubLinhaIdioma.NomeSublinha = '' THEN
		                    SubLinha.NomeSublinha
	                      ELSE
		                    SubLinhaIdioma.NomeSublinha
                          END as NomeSublinha
                          ,SubLinha.CodigoSublinha          
                          ,Linha.NomeLinha
                          ,Linha.CodigoLinha
                          ,Produto.[Ativo]
                          ,Produto.[Preco]
                          ,Produto.[Ativo]
                          ,(
                                SELECT TOP 1 a.Caminho FROM Arquivo a
                                WHERE a.IdPai = Produto.ID AND a.Ordem = 1 AND a.Ativo = 1 AND
                                A.IdTipoArquivo = (Select Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'imgCard')
                            ) AS ImagemUrl
                    FROM [Produto]
                    INNER JOIN SubLinha
                      ON Produto.IdSubLinha = SubLinha.Id
                    INNER JOIN Linha
                      ON SubLinha.IdLinha = Linha.Id
                    LEFT JOIN ProdutoIdioma
                        ON Produto.Id = ProdutoIdioma.IdProduto AND ProdutoIdioma.CodigoIdioma = @idioma
                    LEFT JOIN SubLinhaIdioma
                        ON SubLinha.Id = SubLinhaIdioma.IdSubLinha AND SubLinhaIdioma.CodigoIdioma = @idioma
                    WHERE (Produto.CodigoProduto = @produto OR (@buscaPorId = 1 AND Produto.Id = @idProduto))
                          AND (@ativo IS NULL OR Produto.Ativo = @ativo)
                ", new { produto, buscaPorId, idProduto, idioma, ativo }).FirstOrDefault();
            }
        }

        public List<AparelhoIdealResultado> RetornaAparelhoIdeal(AparelhoIdeal model)
        {
            var constAreaJanela = 2;

            model.altura = 3;

            var areaJanelaManha = model.manhaComSol * constAreaJanela;
            var areaJanelaTarde = model.tardeComSol * constAreaJanela;
            var areaJanelaDiaTodo = model.diaComSol * constAreaJanela;
            var areaJanelaSemSol = model.semSol * constAreaJanela;

            var transmissao = (areaJanelaManha + areaJanelaTarde + areaJanelaDiaTodo + areaJanelaSemSol) * 105;

            var valorCortinaManha = 0;
            var valorCortinaTarde = 0;
            var valorCortinaDiaTodo = 0;
            var valorCortinaSemSol = 0;

            if (model.cortina == 1)
            {
                valorCortinaManha = 550 * areaJanelaManha;
                valorCortinaTarde = 920 * areaJanelaTarde;
                valorCortinaDiaTodo = 480 * areaJanelaDiaTodo;
                valorCortinaSemSol = 0;
            }
            else
            {
                valorCortinaManha = 1130 * areaJanelaManha;
                valorCortinaTarde = 2100 * areaJanelaTarde;
                valorCortinaDiaTodo = 1000 * areaJanelaDiaTodo;
                valorCortinaSemSol = 0;
            }

            var janela = (valorCortinaManha + valorCortinaTarde + valorCortinaDiaTodo + valorCortinaSemSol);

            var area = model.largura * model.comprimento;

            var parede1 = model.largura * model.altura;
            var parede2 = model.comprimento * model.altura;
            var parede3 = model.largura * model.altura;
            var parede4 = model.comprimento * model.altura;

            var constParedeSemsol = 55;
            var constParedeComSol = 84;
            var constParedeInterna = 33;

            var paredeSemsol = (constParedeSemsol * parede1) + (constParedeSemsol * parede2);
            var paredeComSol = (constParedeComSol * parede1) + (constParedeSemsol * parede2);
            var paredeInterna = (parede3 + parede4) * constParedeInterna;

            var semSol = paredeSemsol + paredeInterna;
            var comSol = paredeComSol + paredeInterna; // não usa

            var teto = 0;
            if (model.teto == 1)
            {
                teto = area * 315;
            }
            else if (model.teto == 2)
            {
                teto = area * 55;
            }
            else if (model.teto == 3)
            {
                teto = area * 210;
            }

            var piso = area * 55;

            var pessoas = model.pessoas * 630;

            var incandescente = model.incandescente * 4 * 60;

            var fluerescente = model.fluorescente * 4 * 20;

            var aparelhosEletricos = model.aparelhos * 4 * 120;

            var vaosAbertos = model.aberturas * 630;

            var soma = janela + transmissao + comSol + teto + piso + pessoas + incandescente + fluerescente + aparelhosEletricos + vaosAbertos;
            var valor1 = soma * 0.0;
            var PA = model.estado;

            var resultado = valor1 * PA;
            var total = resultado * 0.0;

           //var lista = ListaAparelhosIdeais();

            var retorno = ListaAparelhosIdeais(total);

            //var BTU = 

            //var somaBTU = (550 * areaJanelaManha) + transmissao + comSol + (area * 55) + piso + pessoas + fluerescente + aparelhosEletricos + vaosAbertos;

            return retorno;
        }

        private List<AparelhoIdealResultado> ListaAparelhosIdeais(double total)
        {
            var retorno = new List<AparelhoIdealResultado>();

            var sql = @"
                    SELECT 
	                    Produto.[Id]
	                    ,Produto.[CodigoProduto]
	                    ,Produto.[CodigoLegado]
	                    ,Produto.[NomeProduto]
	                    ,Produto.[IdSubLinha]
	                    ,Produto.[Ativo]     
                        ,Linha.CodigoLinha
                        ,Sublinha.CodigoSublinha
                        ,Imagem.NomeArquivo
	                    ,Imagem.Id
	                    ,Imagem.Caminho
                    FROM Produto
                    INNER JOIN Sublinha
                    ON Produto.IdSublinha = Sublinha.Id
                    INNER JOIN Linha
                    ON Linha.Id = Sublinha.IdLinha
					INNER JOIN Modelo
					ON Modelo.IdProduto = Produto.Id
                    LEFT JOIN Arquivo Imagem
                    ON Imagem.IdPai = Produto.Id AND Imagem.IdTipoArquivo = (SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'imgCard')
                    WHERE
					Produto.Ativo = 1 AND Modelo.Ativo = 1 AND (Imagem.Ativo IS NULL OR Imagem.Ativo = 1) AND
                    @total between Modelo.AparelhoIdealDe and Modelo.AparelhoIdealAte 
                    ORDER BY
                    Produto.NomeProduto, Modelo.NomeModelo, Imagem.Ordem
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, AparelhoIdealResultado>();

                retorno = conexao.Query<AparelhoIdealResultado, Arquivo, AparelhoIdealResultado>(sql,
                (produto, arquivo) =>
                {
                    AparelhoIdealResultado produtoEntry;

                    if (!dictionary.TryGetValue(produto.Id, out produtoEntry))
                    {
                        produtoEntry = produto;
                        produtoEntry.Arquivos = new List<Arquivo>();
                        dictionary.Add(produtoEntry.Id, produtoEntry);
                    }

                    if (arquivo != null && !produtoEntry.Arquivos.Any(x => x.Id == arquivo.Id))
                    {
                        produtoEntry.Arquivos.Add(arquivo);
                    }

                    return produtoEntry;
                }, new { total },
                splitOn: "NomeArquivo").Distinct().ToList();
            }

            return retorno;
        }

        public List<Secao> RetornaSecoes()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Secao>(
                @"
                  SELECT Secao.[Id]
                        ,Secao.[Nome]
                        ,Secao.[Descricao]
                        ,Secao.[QtdImagens]
                        ,Secao.ExibeTexto1
                        ,Secao.ExibeTexto2
                        ,Secao.ExibeTexto3
                        ,Secao.ExibeCodigoVideo
                        ,Secao.ExibeCodigoVideo2
                        ,Secao.ExibeCodigoVideo3
                        ,Secao.ExibeCodigoVideo4
                        ,Secao.ExibeAparelhoIdeal
                        ,Secao.Codigo
                  FROM [Secao]
                  ORDER BY Nome
                ", new { }).ToList();
            }
        }

        public List<SecaoProduto> RetornaSecoesProduto(Guid idProduto, bool? ativo, int? idioma)
        {
            var retorno = new List<SecaoProduto>();

            var sql = @"
                        SELECT secProd.Id
                             , secProd.IdSecao
                             , sec.Nome AS NomeSecao
                             , sec.Descricao
                             , sec.Nome
                             , sec.Descricao
                             , sec.ExibeTexto1
                             , sec.ExibeTexto2
                             , sec.ExibeTexto3
                             , sec.ExibeCodigoVideo
                             , sec.ExibeCodigoVideo2
                             , sec.ExibeCodigoVideo3
                             , sec.ExibeCodigoVideo4
                             , sec.ExibeAparelhoIdeal
                             , sec.QtdImagens
                             , sec.QtdIcones
                             , sec.Codigo AS CodigoSecao
                             , secProd.IdProduto
                             ,CASE WHEN SecaoProdutoIdioma.Texto1 IS NULL OR SecaoProdutoIdioma.Texto1 = '' OR @idioma IS NULL THEN
		                        secProd.Texto1
	                         ELSE
		                        SecaoProdutoIdioma.Texto1
                             END as Texto1
                             ,CASE WHEN SecaoProdutoIdioma.Texto2 IS NULL OR SecaoProdutoIdioma.Texto2 = '' OR @idioma IS NULL THEN
		                        secProd.Texto2
	                         ELSE
		                        SecaoProdutoIdioma.Texto2
                             END as Texto2
                             ,CASE WHEN SecaoProdutoIdioma.Texto3 IS NULL OR SecaoProdutoIdioma.Texto3 = '' OR @idioma IS NULL THEN
		                        secProd.Texto3
	                         ELSE
		                        SecaoProdutoIdioma.Texto3
                             END as Texto3
                             , secProd.Ordem
                             , secProd.CodigoVideo
                             , secProd.CodigoVideo2
                             , secProd.CodigoVideo3
                             , secProd.CodigoVideo4
                             , secProd.AparelhoIdeal
                             , secProd.Ativo
                             , arq.Caminho AS Caminho
                             ,CASE WHEN arqIdioma.NomeArquivo IS NULL OR arqIdioma.NomeArquivo = '' OR @idioma IS NULL THEN
		                        arq.NomeArquivo
	                         ELSE
		                        arqIdioma.NomeArquivo
                             END as NomeArquivo
                             , arq.Id
                             , arq.Ordem
                             , arq.Ativo
                             , arq.Linque
                             , null DescricaoIcone
                             , null SubDescricaoIcone
                             , null Id
                             , null IdSecaoProduto
                             , null IdIcone
                             , null Ordem
                             , null AS UrlIcone
                          FROM SecaoProduto secProd
		                       INNER JOIN Secao sec
		                       	       ON sec.Id = secProd.IdSecao
		                        LEFT JOIN Arquivo arq
		                       	       ON (arq.IdSecao = secProd.Id AND (@ativo IS NULL OR arq.Ativo = @ativo)) 
                                LEFT JOIN ArquivoIdioma arqIdioma
                                    ON arq.Id = arqIdioma.IdArquivo AND arqIdioma.CodigoIdioma = @idioma
                                LEFT JOIN SecaoProdutoIdioma
                                    ON secProd.Id = SecaoProdutoIdioma.IdSecaoProduto AND SecaoProdutoIdioma.CodigoIdioma = @idioma
                         WHERE secProd.IdProduto = @idProduto AND (@ativo IS NULL OR secProd.Ativo = @ativo)
                        UNION
                        SELECT secProd.Id
                             , secProd.IdSecao
                             , sec.Nome AS NomeSecao
                             , sec.Descricao
                             , sec.Nome
                             , sec.Descricao
                             , sec.ExibeTexto1
                             , sec.ExibeTexto2
                             , sec.ExibeTexto3
                             , sec.ExibeCodigoVideo
                             , sec.ExibeCodigoVideo2
                             , sec.ExibeCodigoVideo3
                             , sec.ExibeCodigoVideo4
                             , sec.ExibeAparelhoIdeal
                             , sec.QtdImagens
                             , sec.QtdIcones
                             , sec.Codigo AS CodigoSecao
                             , secProd.IdProduto
                             ,CASE WHEN SecaoProdutoIdioma.Texto1 IS NULL OR SecaoProdutoIdioma.Texto1 = '' OR @idioma IS NULL THEN
		                        secProd.Texto1
	                         ELSE
		                        SecaoProdutoIdioma.Texto1
                             END as Texto1
                             ,CASE WHEN SecaoProdutoIdioma.Texto2 IS NULL OR SecaoProdutoIdioma.Texto2 = '' OR @idioma IS NULL THEN
		                        secProd.Texto2
	                         ELSE
		                        SecaoProdutoIdioma.Texto2
                             END as Texto2
                             ,CASE WHEN SecaoProdutoIdioma.Texto3 IS NULL OR SecaoProdutoIdioma.Texto3 = '' OR @idioma IS NULL THEN
		                        secProd.Texto3
	                         ELSE
		                        SecaoProdutoIdioma.Texto3
                             END as Texto3
                             , secProd.Ordem
                             , secProd.CodigoVideo
                             , secProd.CodigoVideo2
                             , secProd.CodigoVideo3
                             , secProd.CodigoVideo4
                             , secProd.AparelhoIdeal
                             , secProd.Ativo
                             , arq.Caminho AS Caminho
                             ,CASE WHEN arqIdioma.NomeArquivo IS NULL OR arqIdioma.NomeArquivo = '' OR @idioma IS NULL THEN
		                        arq.NomeArquivo
	                         ELSE
		                        arqIdioma.NomeArquivo
                             END as NomeArquivo
                             , arq.Id
                             , arq.Ordem
                             , arq.Ativo
                             , arq.Linque
                             ,CASE WHEN secIcoIdi.DescricaoIcone IS NULL OR secIcoIdi.DescricaoIcone = '' OR @idioma IS NULL THEN
		                        secProdIco.DescricaoIcone
	                         ELSE
		                        secIcoIdi.DescricaoIcone
                             END as DescricaoIcone
                             ,CASE WHEN secIcoIdi.SubDescricaoIcone IS NULL OR secIcoIdi.SubDescricaoIcone = '' OR @idioma IS NULL THEN
		                        secProdIco.SubDescricaoIcone
	                         ELSE
		                        secIcoIdi.SubDescricaoIcone
                             END as SubDescricaoIcone
                             , secProdIco.Id
                             , secProdIco.IdSecaoProduto
                             , secProdIco.IdIcone
                             , secProdIco.Ordem
                             , arq.Caminho AS UrlIcone
                          FROM SecaoProduto secProd
		                       INNER JOIN Secao sec
		                       	       ON sec.Id = secProd.IdSecao
		                        LEFT JOIN SecaoProdutoIcone secProdIco
		                               ON secProdIco.IdSecaoProduto = secProd.Id
		                        LEFT JOIN Arquivo arq
		                       	       ON (arq.Id = secProdIco.IdIcone AND (@ativo IS NULL OR arq.Ativo = @ativo))
                                LEFT JOIN ArquivoIdioma arqIdioma
                                    ON arq.Id = arqIdioma.IdArquivo AND arqIdioma.CodigoIdioma = @idioma
                                LEFT JOIN SecaoProdutoIdioma
                                    ON secProd.Id = SecaoProdutoIdioma.IdSecaoProduto AND SecaoProdutoIdioma.CodigoIdioma = @idioma
                                LEFT JOIN SecaoProdutoIconeIdioma secIcoIdi
                                    ON secProdIco.Id = secIcoIdi.IdSecaoProdutoIcone AND secIcoIdi.CodigoIdioma = @idioma
                         WHERE secProd.IdProduto = @idProduto AND (@ativo IS NULL OR secProd.Ativo = @ativo)
                        ORDER BY 18, 25, 31
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, SecaoProduto>();

                retorno = conexao.Query<SecaoProduto, Arquivo, SecaoProdutoIcone, SecaoProduto>(sql,
                (secao, arquivo, secaoProdutoIcone) =>
                {
                    SecaoProduto secaoEntry;

                    if (!dictionary.TryGetValue(secao.Id, out secaoEntry))
                    {
                        secaoEntry = secao;
                        secaoEntry.Arquivos = new List<Arquivo>();
                        secaoEntry.Icones = new List<SecaoProdutoIcone>();
                        dictionary.Add(secaoEntry.Id, secaoEntry);
                    }

                    if (arquivo != null && secaoProdutoIcone == null)
                    {
                        secaoEntry.Arquivos.Add(arquivo);
                    }

                    if (secaoProdutoIcone != null)
                    {
                        secaoEntry.Icones.Add(secaoProdutoIcone);
                    }

                    return secaoEntry;

                }, new { idProduto, ativo, idioma },
                splitOn: "Caminho, DescricaoIcone").Distinct().ToList();
            }

            return retorno;
        }

        public List<Produto> RetornaProdutoFiltro(string codigo, int idioma)
        {
            var retorno = new List<Produto>();

            var sql = @"
                    SELECT 
	                    Produto.[Id]
	                    ,Produto.[CodigoProduto]
	                    ,Produto.[CodigoLegado]
                        ,CASE WHEN ProdutoIdioma.NomeProduto IS NULL OR ProdutoIdioma.NomeProduto = '' THEN
		                    Produto.NomeProduto
	                    ELSE
		                    ProdutoIdioma.NomeProduto
	                    END as NomeProduto
	                    ,Produto.[IdSubLinha]
	                    ,Produto.[Ativo]      
                
                        ,Sublinha.[CodigoSublinha]
	                    ,Caracteristica.[DescricaoCaracteristica]
                        ,CASE WHEN CI.DescricaoCaracteristica IS NULL OR CI.DescricaoCaracteristica = '' THEN
		                    Caracteristica.DescricaoCaracteristica
	                    ELSE
		                    CI.DescricaoCaracteristica
	                    END as DescricaoCaracteristica
	                    ,Caracteristica.[Id]
	                    ,Caracteristica.[IdProduto]
	                    ,Caracteristica.[CaracteristicaPrincipal]
	                    ,Caracteristica.[IdIcone]
                        ,Icone.Caminho AS urlIcone
                        ,Classificacao.NomeClassificacao
						,Classificacao.Id   
                        ,Classificacao.IdClassificacaoSuperior
                        ,Classificacao.Comparativo
                        ,Imagem.NomeArquivo
	                    ,Imagem.Id
	                    ,Imagem.Caminho
                        ,ET.NomeEspecificacao
                        ,ET.CodigoEspecificacao
                        ,ET.Comparativo
						,ET.Id						
                        ,CASE WHEN PETI.Valor IS NULL OR PETI.Valor = '' THEN
		                    ISNULL(PET.Valor,'-')
	                    ELSE
		                    ISNULL(PETI.Valor,'-')
	                    END as Valor
                    FROM Produto
                    INNER JOIN Sublinha
                    ON Produto.IdSublinha = Sublinha.Id
					LEFT JOIN ProdutoClassificacao
					ON ProdutoClassificacao.IdProduto = Produto.Id
                    LEFT JOIN Classificacao
                    ON Classificacao.Id = ProdutoClassificacao.IdClassificacao 
                    LEFT JOIN Caracteristica
                    ON Caracteristica.IdProduto = Produto.Id AND Caracteristica.CaracteristicaPrincipal = 1
                    LEFT JOIN Arquivo Icone
                    ON Icone.Id = Caracteristica.IdIcone	
                    LEFT JOIN Arquivo Imagem
                    ON Imagem.IdPai = Produto.Id AND Imagem.IdTipoArquivo = (SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'imgCard')
                    LEFT JOIN TipoArquivo
                    ON TipoArquivo.Id = Imagem.IdTipoArquivo
                    LEFT JOIN EspecificacaoTecnica ET
					ON ET.IdSubLinha = Produto.IdSubLinha AND ET.Comparativo = 1
					LEFT JOIN ProdutoEspecificacaoTecnica PET
					ON PET.IdEspecificacaoTecnica = ET.ID AND PET.IdProduto = Produto.Id 
                    LEFT JOIN ProdutoAparelhoIdeal
                    ON ProdutoAparelhoIdeal.IdProduto = Produto.Id
                    LEFT JOIN ProdutoIdioma
                    ON Produto.Id = ProdutoIdioma.IdProduto AND ProdutoIdioma.CodigoIdioma = @idioma
                    LEFT JOIN CaracteristicaIdioma AS CI
                    ON Caracteristica.Id = CI.IdCaracteristica AND CI.CodigoIdioma = @idioma
                    LEFT JOIN ProdutoEspecificacaoTecnicaIdioma AS PETI
                    ON PETI.IdProdutoEspecificacaoTecnica = CONCAT(Produto.Id, ET.Id) AND PETI.CodigoIdioma = @idioma
                    WHERE
                    Sublinha.CodigoSublinha = @codigo AND Produto.Ativo = 1
                    ORDER BY
                    Produto.NomeProduto, Caracteristica.Ordem, Imagem.Ordem
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, Produto>();

                retorno = conexao.Query<Produto, Caracteristica, Classificacao, Arquivo, EspecificacaoTecnica, Produto>(sql,
                (produto, caracteristica, classificacao, arquivo, especificacao) =>
                {
                    Produto produtoEntry;

                    if (!dictionary.TryGetValue(produto.Id, out produtoEntry))
                    {
                        produtoEntry = produto;
                        produtoEntry.Caracteristicas = new List<Caracteristica>();
                        produtoEntry.Classificacoes = new List<Classificacao>();
                        produtoEntry.Arquivos = new List<Arquivo>();
                        produtoEntry.EspecificacoesTecnicas = new List<EspecificacaoTecnica>();
                        dictionary.Add(produtoEntry.Id, produtoEntry);
                    }

                    if (caracteristica != null && !produtoEntry.Caracteristicas.Any(x => x.Id == caracteristica.Id) && produtoEntry.Caracteristicas.Count < 3)
                    {
                        produtoEntry.Caracteristicas.Add(caracteristica);
                    }

                    if (classificacao != null && !produtoEntry.Classificacoes.Any(x => x.Id == classificacao.Id))
                    {
                        produtoEntry.Classificacoes.Add(classificacao);
                    }

                    if (arquivo != null && !produtoEntry.Arquivos.Any(x => x.Id == arquivo.Id))
                    {
                        produtoEntry.Arquivos.Add(arquivo);
                    }

                    if (especificacao != null && !produtoEntry.EspecificacoesTecnicas.Any(x => x.Id == especificacao.Id))
                    {
                        produtoEntry.EspecificacoesTecnicas.Add(especificacao);
                    }

                    return produtoEntry;
                }, new { codigo, idioma },
                splitOn: "DescricaoCaracteristica,NomeClassificacao,NomeArquivo,NomeEspecificacao").Distinct().ToList();
            }

            return retorno;
        }

        public List<Produto> RetornaProdutoPorCodigoLinha(string codigoLinha)
        {
            var retorno = new List<Produto>();

            var sql = @"
                    SELECT 
	                    Produto.[Id]
	                    ,Produto.[CodigoProduto]
	                    ,Produto.[CodigoLegado]
	                    ,Produto.[NomeProduto]
	                    ,Produto.[IdSubLinha]
	                    ,Produto.[Ativo]
                        ,Produto.Preco
                        ,Sublinha.NomeSublinha AS NomeSublinha
                        ,Imagem.NomeArquivo
	                    ,Imagem.Id
	                    ,Imagem.Caminho
                        ,ET.NomeEspecificacao
                        ,ET.CodigoEspecificacao
                        ,ET.Comparativo
						,ET.Id						
						,PET.Valor as Valor
                    FROM Produto
                    INNER JOIN Sublinha
                    ON Produto.IdSublinha = Sublinha.Id
                    LEFT JOIN Arquivo Imagem
                    ON Imagem.IdPai = Produto.Id AND Imagem.IdTipoArquivo = (SELECT Id FROM TipoArquivo WHERE CodigoTipoArquivo = 'imgCard')
                    LEFT JOIN TipoArquivo
                    ON TipoArquivo.Id = Imagem.IdTipoArquivo
                    LEFT JOIN EspecificacaoTecnica ET
					ON ET.IdSubLinha = Produto.IdSubLinha AND ET.Ativo = 1
					LEFT JOIN ProdutoEspecificacaoTecnica PET
					ON PET.IdEspecificacaoTecnica = ET.ID AND PET.IdProduto = Produto.Id
                    WHERE
                    Sublinha.IdLinha = (SELECT Id FROM Linha WHERE CodigoLinha = @codigoLinha)
                    AND Produto.Ativo = 1
                    ORDER BY
                    Produto.NomeProduto, Imagem.Ordem
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, Produto>();

                retorno = conexao.Query<Produto, Arquivo, EspecificacaoTecnica, Produto>(sql,
                (produto, arquivo, especificacao) =>
                {
                    Produto produtoEntry;

                    if (!dictionary.TryGetValue(produto.Id, out produtoEntry))
                    {
                        produtoEntry = produto;
                        produtoEntry.Arquivos = new List<Arquivo>();
                        produtoEntry.EspecificacoesTecnicas = new List<EspecificacaoTecnica>();
                        dictionary.Add(produtoEntry.Id, produtoEntry);
                    }

                    if (arquivo != null && !produtoEntry.Arquivos.Any(x => x.Id == arquivo.Id))
                    {
                        produtoEntry.Arquivos.Add(arquivo);
                    }

                    if (especificacao != null && !produtoEntry.EspecificacoesTecnicas.Any(x => x.Id == especificacao.Id))
                    {
                        produtoEntry.EspecificacoesTecnicas.Add(especificacao);
                    }

                    return produtoEntry;
                }, new { codigoLinha },
                splitOn: "NomeArquivo,NomeEspecificacao").Distinct().ToList();
            }

            return retorno;
        }

        public SecaoProduto AddSecaoProduto(SecaoProduto model)
        {
            model.Id = Guid.NewGuid();
            //model.Texto1 = "";
            //model.Texto2 = "";
            //model.Texto3 = "";


            var sql = $@"
                        INSERT INTO SecaoProduto
                                   (Id
                                   ,IdSecao
                                   ,IdProduto
                                   ,Texto1
                                   ,Texto2
                                   ,Texto3
                                   ,CodigoVideo
                                   ,CodigoVideo2
                                   ,CodigoVideo3
                                   ,CodigoVideo4
                                   ,AparelhoIdeal
                                   ,Ordem
                                   ,Ativo
                                   ,IdSecaoModelo)
                             VALUES
                                   (
                                   @Id
                                   ,@IdSecao
                                   ,@IdProduto
                                   ,@Texto1
                                   ,@Texto2
                                   ,@Texto3
                                   ,@CodigoVideo
                                   ,@CodigoVideo2
                                   ,@CodigoVideo3
                                   ,@CodigoVideo4
                                   ,@AparelhoIdeal
                                   ,@Ordem
                                   ,@Ativo
                                   ,@IdSecaoModelo)


                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(sql, model);

            }

            return model;
        }

        public Arquivo AddImagemSecaoProduto(Arquivo model)
        {
            var idTipoArquivo = arquivoService.GetIdPorCodigo(model.CodigoTipoArquivo);

            model.IdTipoArquivo = idTipoArquivo;

            arquivoService.AddArquivo(model);

            return model;
        }

        public bool DelSecaoProduto(string id)
        {
            var sql = $@"
                            DELETE FROM SecaoProduto    
                            WHERE Id = @id
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Query<Linha>(sql, new { id });
            }

            return true;
        }
        public Produto SalvarNovoProduto(Produto produto)
        {
            produto.Id = Guid.NewGuid();

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Produto>(@"
                    INSERT INTO Produto
                           (Id
                           ,CodigoProduto
                           ,CodigoLegado
                           ,NomeProduto
                           ,IdSubLinha
                           ,Ativo)
                    VALUES
                           (@Id
                           ,@CodigoProduto
                           ,@CodigoLegado
                           ,@NomeProduto
                           ,@IdSubLinha
                           ,1)

                    SELECT 
						p.Id,
						NomeProduto,
						CodigoProduto,
						p.CodigoLegado,
						IdSubLinha,
						l.Id AS IdLinha
					FROM Produto AS p
					INNER JOIN SubLinha s ON s.Id = p.IdSubLinha
					INNER JOIN Linha l ON l.Id = s.IdLinha
					WHERE P.Id = @Id                    
                    ", produto).First();

            }
        }

        private SolarVariaveis getSolarVariaveis()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<SolarVariaveis>(@"
                    SELECT PerdaSistema, PerdaPlaca, MediaConsumo FROM SolarVariaveis                
                    ").First();
            }
        }

        private List<SolarPotenciaPlacas> getSolarPotenciaPlacas()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<SolarPotenciaPlacas>(@"
                    SELECT Potencia, Placas FROM SolarPotenciaPlacas                
                    ").ToList();
            }
        }

        public List<Produto> RetornaTodosProdutos(string termoPesquisado)
        {
            List<Produto> retorno;

            if (string.IsNullOrEmpty(termoPesquisado) == false)
            {
                termoPesquisado = "%" + termoPesquisado + "%";
            }

            var sql = @"SELECT Produto.Id
                              ,Produto.CodigoLegado
                              ,NomeProduto
                              ,IdSubLinha
                              ,Produto.Ativo
                              ,CodigoProduto
                        FROM Produto
                        INNER JOIN SubLinha ON
                        SubLinha.Id = Produto.IdSubLinha
                        INNER JOIN Linha ON
                        Linha.Id = SubLinha.IdLinha
                        WHERE Produto.Ativo = 1 AND
	                          (Produto.NomeProduto like @termoPesquisado 
	                        OR SubLinha.NomeSubLinha like @termoPesquisado
	                        OR Linha.NomeLinha like @termoPesquisado
	                        OR Produto.CodigoLegado like @termoPesquisado
	                        OR @termoPesquisado is null)
                        ORDER BY Produto.NomeProduto";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Produto>(sql, new { termoPesquisado }).ToList();
            }

            return retorno;
        }
        public SecaoModelo AddSecaoModelo(SecaoModelo model)
        {
            model.Id = Guid.NewGuid();
            model.Ativo = true;
            //model.Texto1 = "";
            //model.Texto2 = "";
            //model.Texto3 = "";


            var sql = $@"
                        INSERT INTO SecaoModelo
                                   (Id
                                   ,IdSecao
                                   ,IdSecaoModeloGrupo
                                   ,Texto1
                                   ,Texto2
                                   ,Texto3
                                   ,Ordem
                                   ,Ativo)
                             VALUES
                                   (
                                   @Id
                                   ,@IdSecao
                                   ,@IdSecaoModeloGrupo
                                   ,@Texto1
                                   ,@Texto2
                                   ,@Texto3
                                   ,@Ordem
                                   ,@Ativo)


                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(sql, model);

            }

            return model;
        }

        public Arquivo AddImagemSecaoModelo(Arquivo model)
        {
            var idTipoArquivo = arquivoService.GetIdPorCodigo(model.CodigoTipoArquivo);

            model.IdTipoArquivo = idTipoArquivo;

            arquivoService.AddArquivo(model);

            return model;
        }

        public bool DelSecaoModelo(string id)
        {
            var sql = $@"
                            DELETE FROM SecaoModelo   
                            WHERE Id = @id
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Query<Linha>(sql, new { id });
            }

            return true;
        }
        private void SalvarSecaoModeloIcone(SecaoModelo model)
        {
            var icones = new List<SecaoModeloIcone>();


            //if (model.CodigoSecao == "secao4")
            //{
            icones.AddRange(model.Icones.Where(x => x.Id == Guid.Empty));
            //}


            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        INSERT INTO [dbo].[SecaoModeloIcone]
                                    ([DescricaoIcone]
                                    ,[IdSecaoModelo]
                                    ,[IdIcone]
                                    ,[Ordem]
                                    ,[SubDescricaoIcone])
                                VALUES
                                    (@DescricaoIcone
                                    ,@IdSecaoModelo
                                    ,@IdIcone
                                    ,@Ordem
                                    ,@SubDescricaoIcone)

                ", icones);
            }
        }
        private void SalvarSecaoProdutoIcone(SecaoProdutoIcone model)
        {

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        INSERT INTO [dbo].[SecaoProdutoIcone]
                                    ([DescricaoIcone]
                                    ,[IdSecaoProduto]
                                    ,[IdIcone]
                                    ,[Ordem]
                                    ,[SubDescricaoIcone])
                                VALUES
                                    (@DescricaoIcone
                                    ,@IdSecaoProduto
                                    ,@IdIcone
                                    ,@Ordem
                                    ,@SubDescricaoIcone)

                ", model);
            }
        }
        private void RemoverSecaoProdutoIcone(Guid IdSecaoProduto)
        {



            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [SecaoProdutoIcone]
                         WHERE  IdSecaoProduto = @IdSecaoProduto
                ", new
                {
                    IdSecaoProduto = IdSecaoProduto
                });
            }

        }
        private void RemoverSecaoModeloIcone(Guid IdSecaoModelo)
        {



            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [SecaoModeloIcone]
                         WHERE  IdSecaoModelo = @IdSecaoModelo
                ", new
                {
                    IdSecaoModelo = IdSecaoModelo
                });
            }

        }
        private void AtualizarSecaoModeloIcone(Produto model)
        {
            var icones = new List<SecaoModeloIcone>();
            foreach (var item in model.SecoesModelo)
            {
                //if (item.CodigoSecao == "secao4")
                //{
                icones.AddRange(item.Icones.Where(x => x.Id != Guid.Empty));
                //}
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE [dbo].[SecaoModeloIcone]
                       SET 
                           [DescricaoIcone] = @DescricaoIcone
                          ,[IdIcone] = @IdIcone
                          ,[Ordem] = @Ordem
                     WHERE Id = @Id

                ", icones);
            }
        }
        private void RemoverSecaoModeloIcone(Produto model)
        {
            foreach (var item in model.SecoesModelo)
            {
                if (item.Icones == null)
                {
                    continue;
                }

                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Execute(
                    @"
                        DELETE [SecaoModeloIcone]
                         WHERE Id NOT IN @Ids AND IdSecaoModelo = @IdSecaoModelo
                ", new
                    {
                        Ids = item.Icones.Select(x => x.Id),
                        IdSecaoModelo = item.Id
                    });
                }
            }
        }
        public void AtualizarSecoesModelo(SecaoModelo model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE [SecaoModelo]
                       SET [Texto1] = @Texto1
                          ,[Texto2] = @Texto2
                          ,[Texto3] = @Texto3
                          ,[CodigoVideo] = @CodigoVideo
                          ,[CodigoVideo2] = @CodigoVideo2
                          ,[CodigoVideo3] = @CodigoVideo3
                          ,[CodigoVideo4] = @CodigoVideo4
                          ,[AparelhoIdeal] = @AparelhoIdeal
                          ,[Ordem] = @Ordem
                          ,[Ativo] = @Ativo
                         WHERE Id = @Id

                ", model);
            }
            if (model.Icones.Count != 0)
            {
                RemoverSecaoModeloIcone(model.Id);
                foreach (SecaoModeloIcone _ico in model.Icones)
                {

                    _ico.IdSecaoModelo = model.Id;
                    _ico.Id = Guid.Empty;
                }
                SalvarSecaoModeloIcone(model);
            }
            if (model.Arquivos.Count != 0)
            {

                arquivoService.UpdateArquivo(model.Arquivos);
            }
        }
        private void RemoverSecoesModelo(SecaoModelo model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [SecaoModelo]
                         WHERE Id = @Id
                ", new
                {
                    model.Id
                });
            }
        }
        public List<SecaoModelo> RetornaSecoesModelo(Guid IdSecaoModeloGrupo, bool? ativo)
        {
            var retorno = new List<SecaoModelo>();

            var sql = @"
                        SELECT secProd.Id
                             , secProd.IdSecao
                             , sec.Nome AS NomeSecao
                             , secProd.IdSecaoModeloGrupo
                             , sec.Descricao
                             , sec.Nome
                             , sec.Descricao
                             , sec.ExibeTexto1
                             , sec.ExibeTexto2
                             , sec.ExibeTexto3
                             , sec.ExibeCodigoVideo
                             , sec.ExibeCodigoVideo2
                             , sec.ExibeCodigoVideo3
                             , sec.ExibeCodigoVideo4
                             , sec.ExibeAparelhoIdeal
                             , sec.QtdImagens
                             , sec.QtdIcones
                             , sec.Codigo AS CodigoSecao
                             , secProd.Id
                             , secProd.Texto1
                             , secProd.Texto2
                             , secProd.Texto3
                             , secProd.Ordem
                             , secProd.CodigoVideo
                             , secProd.AparelhoIdeal
                             , secProd.Ativo
                             , arq.Caminho AS Caminho
                             , arq.NomeArquivo
                             , arq.Id
                             , arq.Ordem
                             , arq.Ativo
                             , null DescricaoIcone
                             , null SubDescricaoIcone
                             , null Id
                             , null IdSecaoModelo
                             , null IdIcone
                             , null Ordem
                             , null AS UrlIcone
                          FROM SecaoModelo secProd
		                       INNER JOIN Secao sec
		                       	       ON sec.Id = secProd.IdSecao
		                        LEFT JOIN Arquivo arq
		                       	       ON (arq.IdSecao = secProd.Id AND (@ativo IS NULL OR arq.Ativo = @ativo)) 
                         WHERE secProd.IdSecaoModeloGrupo = @IdSecaoModeloGrupo AND (@ativo IS NULL OR secProd.Ativo = @ativo)
                        UNION
                        SELECT secProd.Id
                             , secProd.IdSecao
                             , sec.Nome AS NomeSecao
                             , secProd.IdSecaoModeloGrupo
                             , sec.Descricao
                             , sec.Nome
                             , sec.Descricao
                             , sec.ExibeTexto1
                             , sec.ExibeTexto2
                             , sec.ExibeTexto3
                             , sec.ExibeCodigoVideo
                             , sec.ExibeCodigoVideo2
                             , sec.ExibeCodigoVideo3
                             , sec.ExibeCodigoVideo4
                             , sec.ExibeAparelhoIdeal
                             , sec.QtdImagens
                             , sec.QtdIcones
                             , sec.Codigo AS CodigoSecao
                             , secProd.Id
                             , secProd.Texto1
                             , secProd.Texto2
                             , secProd.Texto3
                             , secProd.Ordem
                             , secProd.CodigoVideo
                             , secProd.CodigoVideo2
                             , secProd.CodigoVideo3
                             , secProd.CodigoVideo4
                             , secProd.AparelhoIdeal
                             , secProd.Ativo
                             , arq.Caminho AS Caminho
                             , arq.NomeArquivo
                             , arq.Id
                             , arq.Ordem
                             , arq.Ativo
                             , secProdIco.DescricaoIcone
                             , secProdIco.SubDescricaoIcone
                             , secProdIco.Id
                             , secProdIco.IdSecaoModelo
                             , secProdIco.IdIcone
                             , secProdIco.Ordem
                             , arq.Caminho AS UrlIcone
                          FROM SecaoModelo secProd
		                       INNER JOIN Secao sec
		                       	       ON sec.Id = secProd.IdSecao
		                        LEFT JOIN SecaoModeloIcone secProdIco
		                               ON secProdIco.IdSecaoModelo = secProd.Id
		                        LEFT JOIN Arquivo arq
		                       	       ON (arq.Id = secProdIco.IdIcone AND (@ativo IS NULL OR arq.Ativo = @ativo))
                         WHERE secProd.IdSecaoModeloGrupo = @IdSecaoModeloGrupo AND (@ativo IS NULL OR secProd.Ativo = @ativo)
                        ORDER BY 18, 25, 31
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, SecaoModelo>();

                retorno = conexao.Query<SecaoModelo, Arquivo, SecaoModeloIcone, SecaoModelo>(sql,
                (secao, arquivo, secaoModeloIcone) =>
                {
                    SecaoModelo secaoEntry;

                    if (!dictionary.TryGetValue(secao.Id, out secaoEntry))
                    {
                        secaoEntry = secao;
                        secaoEntry.Arquivos = new List<Arquivo>();
                        secaoEntry.Icones = new List<SecaoModeloIcone>();
                        dictionary.Add(secaoEntry.Id, secaoEntry);
                    }

                    if (arquivo != null && secaoModeloIcone == null)
                    {
                        secaoEntry.Arquivos.Add(arquivo);
                    }

                    if (secaoModeloIcone != null)
                    {
                        secaoEntry.Icones.Add(secaoModeloIcone);
                    }

                    return secaoEntry;

                }, new { IdSecaoModeloGrupo, ativo },
                splitOn: "Caminho, DescricaoIcone").Distinct().ToList();
            }

            return retorno;
        }

        public List<SecaoModeloGrupo> RetornaSecoesModeloGrupo()
        {
            var retorno = new List<SecaoModeloGrupo>();


            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<SecaoModeloGrupo>(
  @"
                   SELECT Id, Descricao, Ativo
                         from SecaoModeloGrupo
                            order by Descricao
                ", new { }).ToList();
            }

            return retorno;

        }


        public SecaoModeloGrupo RetornaSecoesModeloGrupo(Guid id)
        {
            var retorno = new SecaoModeloGrupo();


            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<SecaoModeloGrupo>(
  @"
                   SELECT Id, Descricao, Ativo
                         from SecaoModeloGrupo
                        where id = @Id
                            order by Descricao
                ", new { Id = id }).FirstOrDefault();
            }

            return retorno;

        }

        public SecaoModeloGrupo AddSecaoModeloGrupo(SecaoModeloGrupo model)
        {
            model.Id = Guid.NewGuid();



            var sql = $@"
                        INSERT INTO SecaoModeloGrupo
                                   (Id
                                   ,Descricao
                                   ,Ativo)
                             VALUES
                                   (
                                   @Id
                                   ,@Descricao
                                   ,@Ativo)

                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(sql, model);

            }

            return model;
        }

        private void AtualizarSecoesModeloGrupo(SecaoModeloGrupo model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE [SecaoModeloGrupo]
                       SET [Id] = @Id
                          ,[Descricao] = @Descricao
                          ,[Ativo] = @Ativo
                         WHERE Id = @Id

                ", model);
            }
        }

        public void SalvarSecaoModeloGrupo(SecaoModeloGrupo model)
        {
            if (model.IdProdutos is null)
            {
                List<SecaoModelo> _secaoModelo = RetornaSecaoModelo(model.Id, true);
                List<SecaoProduto> _excluirSecoesProduto = RetornaSecoesProdutoModelo(model.Id, true);

                var IdProdutosAux = new List<Guid>();

                //Remove SecaoProdutos Anteriores
                foreach (SecaoProduto _excluirSecaoProduto in _excluirSecoesProduto)
                {
                    IdProdutosAux.Add(_excluirSecaoProduto.IdProduto);
                    RemoverSecaoProduto(_excluirSecaoProduto);
                    RemoverSecaoProdutoIcone(_excluirSecaoProduto.Id);
                }
                var IdProdutos = IdProdutosAux.Distinct();

                //Add novos SecaoProdutos
                foreach (Guid Id in IdProdutos)
                {
                    foreach (SecaoModelo _secao in _secaoModelo)
                    {
                        var _secaoProduto = new SecaoProduto()
                        {
                            Id = Guid.NewGuid(),
                            IdSecao = _secao.IdSecao,
                            IdSecaoModelo = _secao.Id,
                            IdProduto = Id,
                            Texto1 = _secao.Texto1,
                            Texto2 = _secao.Texto2,
                            Texto3 = _secao.Texto3,
                            CodigoVideo = _secao.CodigoVideo,
                            CodigoVideo2 = _secao.CodigoVideo2,
                            CodigoVideo3 = _secao.CodigoVideo3,
                            CodigoVideo4 = _secao.CodigoVideo4,
                            AparelhoIdeal = _secao.AparelhoIdeal,
                            Ativo = true
                        };
                        AddSecaoProduto(_secaoProduto);

                        List<Arquivo> _arquivoModelo = arquivoService.GetArquivoSecao(_secaoProduto.IdSecaoModelo);
                        List<SecaoModeloIcone> _icones = RetornaSecoesModeloIcone(_secaoProduto.IdSecaoModelo);



                        foreach (Arquivo _arquivoProd in _arquivoModelo)
                        {
                            _arquivoProd.Id = Guid.Empty;
                            _arquivoProd.IdSecao = _secaoProduto.Id;

                            arquivoService.AddArquivo(_arquivoProd);
                        }
                        foreach (SecaoModeloIcone _ico in _icones)
                        {
                            SecaoProdutoIcone _prodIco = new SecaoProdutoIcone()
                            {
                                Id = Guid.NewGuid(),
                                DescricaoIcone = _ico.DescricaoIcone,
                                SubDescricaoIcone = _ico.SubDescricaoIcone,
                                IdSecaoProduto = _secaoProduto.Id,
                                IdIcone = _ico.IdIcone,
                                UrlIcone = _ico.UrlIcone,
                                Ordem = _ico.Ordem
                  

                            };

                            SalvarSecaoProdutoIcone(_prodIco);
                        }
                    }
                }
            }
            else
            {
                List<SecaoModelo> _secaoModelo = RetornaSecaoModelo(model.Id, true);
                List<SecaoProduto> _excluirSecoesProduto = new List<SecaoProduto>();

                foreach (SecaoModeloGrupoProduto _prod in model.IdProdutos)
                {
                    Guid IdProduto = new Guid(_prod.Id);
                    _excluirSecoesProduto = RetornaSecoesProduto(IdProduto, true, null);

                    foreach (SecaoProduto _exclude in _excluirSecoesProduto)
                    {
                        RemoverSecaoProduto(_exclude);
                        RemoverSecaoProdutoIcone(_exclude.Id);
                    }

                    if (_prod.Ativo == true)
                    {
                        foreach (SecaoModelo _secao in _secaoModelo)
                        {

                            var _secaoProduto = new SecaoProduto()
                            {
                                Id = Guid.NewGuid(),
                                IdSecao = _secao.IdSecao,
                                IdSecaoModelo = _secao.Id,
                                IdProduto = IdProduto,
                                Texto1 = _secao.Texto1,
                                Texto2 = _secao.Texto2,
                                Texto3 = _secao.Texto3,
                                CodigoVideo = _secao.CodigoVideo,
                                CodigoVideo2 = _secao.CodigoVideo2,
                                CodigoVideo3 = _secao.CodigoVideo3,
                                CodigoVideo4 = _secao.CodigoVideo4,
                                AparelhoIdeal = _secao.AparelhoIdeal,
                                Ordem = _secao.Ordem,
                                Ativo = true
                            };
                            AddSecaoProduto(_secaoProduto);

                            List<Arquivo> _arquivoModelo = arquivoService.GetArquivoSecao(_secaoProduto.IdSecaoModelo);
                            List<SecaoModeloIcone> _icones = RetornaSecoesModeloIcone(_secaoProduto.IdSecaoModelo);

                            foreach (Arquivo _arquivoProd in _arquivoModelo)
                            {
                                _arquivoProd.Id = Guid.Empty;
                                _arquivoProd.IdSecao = _secaoProduto.Id;

                                arquivoService.AddArquivo(_arquivoProd);
                            }
                            foreach (SecaoModeloIcone _ico in _icones)
                            {
                                SecaoProdutoIcone _prodIco = new SecaoProdutoIcone()
                                {
                                    Id = Guid.NewGuid(),
                                    DescricaoIcone = _ico.DescricaoIcone,
                                    SubDescricaoIcone = _ico.SubDescricaoIcone,
                                    IdSecaoProduto = _secaoProduto.Id,
                                    IdIcone = _ico.IdIcone,
                                    UrlIcone = _ico.UrlIcone,
                                    Ordem = _ico.Ordem
                              

                                };
                                SalvarSecaoProdutoIcone(_prodIco);

                            }
                        }
                    }
                }
            }

            //return model;
        }
        public List<SecaoModelo> RetornaSecaoModelo(Guid IdSecaoModeloGrupo, bool? Ativo)
        {
            var retorno = new List<SecaoModelo>();

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {

                retorno = conexao.Query<SecaoModelo>(
                   @"
                        select
                            Id,
                            IdSecao,
                            IdSecaoModeloGrupo,
                            Texto1,
                            Texto2,
                            Texto3,
                            CodigoVideo,
                            CodigoVideo2,
                            CodigoVideo3,
                            CodigoVideo4,
                            AparelhoIdeal,
                            Ordem,
                            Ativo
                            
                             from SecaoModelo

                         WHERE IdSecaoModeloGrupo = @IdSecaoModeloGrupo AND (@ativo IS NULL OR Ativo = @ativo)
                        
                ", new { IdSecaoModeloGrupo, Ativo }).ToList();
            }

            return retorno;
        }
        public List<SecaoProduto> RetornaSecoesProdutoModelo(Guid IdSecaoModelo, bool? ativo)
        {
            var retorno = new List<SecaoProduto>();

            var sql = @"
                        SELECT secProd.Id
                             , secProd.IdSecao
							 ,secProd.IdSecaoModelo
                             , sec.Nome AS NomeSecao
                             , sec.Descricao
                             , sec.Nome
                             , sec.Descricao
                             , sec.ExibeTexto1
                             , sec.ExibeTexto2
                             , sec.ExibeTexto3
                             , sec.ExibeCodigoVideo
                             , sec.ExibeCodigoVideo2
                             , sec.ExibeCodigoVideo3
                             , sec.ExibeCodigoVideo4
                             , sec.ExibeAparelhoIdeal
                             , sec.QtdImagens
                             , sec.QtdIcones
                             , sec.Codigo AS CodigoSecao
                             , secProd.IdProduto
                             , secProd.Texto1
                             , secProd.Texto2
                             , secProd.Texto3
                             , secProd.Ordem
                             , secProd.CodigoVideo
                             , secProd.CodigoVideo2
                             , secProd.CodigoVideo3
                             , secProd.CodigoVideo4
                             , secProd.AparelhoIdeal
                             , secProd.Ativo
                             , arq.Caminho AS Caminho
                             , arq.NomeArquivo
                             , arq.Id
                             , arq.Ordem
                             , arq.Ativo
                             , null DescricaoIcone
                             , null SubDescricaoIcone
                             , null Id
                             , null IdSecaoProduto
                             , null IdIcone
                             , null Ordem
                             , null AS UrlIcone
                          FROM SecaoProduto secProd

		                       INNER JOIN Secao sec  ON sec.Id = secProd.IdSecao
							   LEFT JOIN Arquivo arq ON (arq.IdSecao = secProd.Id AND (@ativo IS NULL OR arq.Ativo = @ativo)) 

                         WHERE secProd.IdSecaoModelo = @IdSecaoModelo AND (@ativo IS NULL OR secProd.Ativo = @ativo)

                        UNION
                        SELECT secProd.Id
                             , secProd.IdSecao
							 ,secProd.IdSecaoModelo
                             , sec.Nome AS NomeSecao
                             , sec.Descricao
                             , sec.Nome
                             , sec.Descricao
                             , sec.ExibeTexto1
                             , sec.ExibeTexto2
                             , sec.ExibeTexto3
                             , sec.ExibeCodigoVideo
                             , sec.ExibeCodigoVideo2
                             , sec.ExibeCodigoVideo3
                             , sec.ExibeCodigoVideo4
                             , sec.ExibeAparelhoIdeal
                             , sec.QtdImagens
                             , sec.QtdIcones
                             , sec.Codigo AS CodigoSecao
                             , secProd.IdProduto
                             , secProd.Texto1
                             , secProd.Texto2
                             , secProd.Texto3
                             , secProd.Ordem
                             , secProd.CodigoVideo
                             , secProd.CodigoVideo2
                             , secProd.CodigoVideo3
                             , secProd.CodigoVideo4
                             , secProd.AparelhoIdeal
                             , secProd.Ativo
                             , arq.Caminho AS Caminho
                             , arq.NomeArquivo
                             , arq.Id
                             , arq.Ordem
                             , arq.Ativo
                             , secProdIco.DescricaoIcone
                             , secProdIco.SubDescricaoIcone
                             , secProdIco.Id
                             , secProdIco.IdSecaoProduto
                             , secProdIco.IdIcone
                             , secProdIco.Ordem
                             , arq.Caminho AS UrlIcone
                          FROM SecaoProduto secProd

		                       INNER JOIN Secao sec ON sec.Id = secProd.IdSecao
							   LEFT JOIN SecaoProdutoIcone secProdIco ON secProdIco.IdSecaoProduto = secProd.Id
		                       LEFT JOIN Arquivo arq ON (arq.Id = secProdIco.IdIcone AND (@ativo IS NULL OR arq.Ativo = @ativo))

                         WHERE secProd.IdSecaoModelo = @IdSecaoModelo AND (@ativo IS NULL OR secProd.Ativo = @ativo)
                        ORDER BY 18, 25, 31
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, SecaoProduto>();

                retorno = conexao.Query<SecaoProduto, Arquivo, SecaoProdutoIcone, SecaoProduto>(sql,
                (secao, arquivo, secaoProdutoIcone) =>
                {
                    SecaoProduto secaoEntry;

                    if (!dictionary.TryGetValue(secao.Id, out secaoEntry))
                    {
                        secaoEntry = secao;
                        secaoEntry.Arquivos = new List<Arquivo>();
                        secaoEntry.Icones = new List<SecaoProdutoIcone>();
                        dictionary.Add(secaoEntry.Id, secaoEntry);
                    }

                    if (arquivo != null && secaoProdutoIcone == null)
                    {
                        secaoEntry.Arquivos.Add(arquivo);
                    }

                    if (secaoProdutoIcone != null)
                    {
                        secaoEntry.Icones.Add(secaoProdutoIcone);
                    }

                    return secaoEntry;

                }, new { IdSecaoModelo, ativo },
                splitOn: "Caminho, DescricaoIcone").Distinct().ToList();
            }

            return retorno;
        }

        public List<Produto> RetornaSecoesModeloGrupoProduto(Guid IdSecaoModeloGrupo)
        {
            var retorno = new List<Produto>();

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Produto>(
  @"
                    select 		Distinct(a.[Id])
                                 ,a.[CodigoLegado]
                                 ,a.[NomeProduto]
                                 ,a.[IdSubLinha]
                                 ,a.[Ativo]
                                 ,a.[CodigoProduto] from Produto a

                    inner join SecaoProduto b on b.IdProduto = a.Id
					inner join SecaoModelo c on c.Id = b.IdSecaoModelo
                where c.IdSecaoModeloGrupo = @IdSecaoModeloGrupo
                           
                ", new { IdSecaoModeloGrupo = IdSecaoModeloGrupo }).ToList();
            }

            return retorno;

        }

        public List<SecaoModeloIcone> RetornaSecoesModeloIcone(Guid IdSecaoModelo)
        {
            var retorno = new List<SecaoModeloIcone>();


            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<SecaoModeloIcone>(
  @"
                            select 	a.[Id]
                                 ,a.[DescricaoIcone]
                                 ,a.[IdSecaoModelo]
                                 ,a.[IdIcone]
                                 ,a.[Ordem]
                                 ,a.[SubDescricaoIcone] from SecaoModeloIcone a

         
                where a.IdSecaoModelo = @IdSecaoModelo
                           
                ", new { IdSecaoModelo = IdSecaoModelo }).ToList();
            }

            return retorno;

        }

        public bool CopiarProduto(Produto produtoOrigem, string idProdutoDestino)
        {
            try
            {
                var produtoDestino = PreencheProduto(idProdutoDestino, null, null);

                produtoDestino.Classificacoes = produtoOrigem.Classificacoes;
                produtoDestino.EspecificacoesTecnicas = produtoOrigem.EspecificacoesTecnicas;

                RemoverEspecificacaoTecnica(produtoDestino);
                SalvarEspecificacaoTecnica(produtoDestino);

                RemoverClassificacoes(produtoDestino);
                SalvarClassificacoes(produtoDestino);

            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public List<Produto> RetornarProdutosPorCodigoLinha(string codigoLinha)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Produto>(
                 @"
                    SELECT 
	                    Produto.Id, 
	                    Produto.NomeProduto
                    FROM Produto
                    JOIN Sublinha
                    ON Produto.IdSublinha = Sublinha.Id
                    WHERE IdLinha = (SELECT Id FROM Linha WHERE codigoLinha =  @codigoLinha)
                ", new { codigoLinha }).ToList();
            }
        }

    }
}
