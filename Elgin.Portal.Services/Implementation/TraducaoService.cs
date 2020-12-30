using Elgin.Portal.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using Elgin.Portal.Services.Model;
using System.Linq;
using System;
using System.Transactions;

namespace Elgin.Portal.Services.Implementation
{
    public class TraducaoService : AbstractService<TraducaoService>
    {
        public TraducaoService(string connectionString) : base(connectionString) { }

		public bool Salvar(List<Traducao> modelList)
		{
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					foreach (var model in modelList)
					{
					
						using (SqlConnection conexao = new SqlConnection(ConnectionString))
						{
							if (model.en == null) model.en = "";
							if (model.es == null) model.es = "";

							var sql = $@"
							 IF NOT EXISTS(SELECT 1 FROM {model.tabela} WHERE {model.campoRef} = @Id AND CodigoIdioma = 2) INSERT INTO {model.tabela}({model.campoRef}, CodigoIdioma, {model.campo}) VALUES (@id, 2, @en) 
							 ELSE UPDATE {model.tabela} SET {model.campo} = @en WHERE {model.campoRef} = @Id AND CodigoIdioma = 2

							 IF NOT EXISTS(SELECT 1 FROM {model.tabela} WHERE {model.campoRef} = @Id AND CodigoIdioma = 3) INSERT INTO {model.tabela}({model.campoRef}, CodigoIdioma, {model.campo}) VALUES (@id, 3, @es) 
							 ELSE UPDATE {model.tabela} SET {model.campo} = @es WHERE {model.campoRef} = @Id AND CodigoIdioma = 3
							";

							conexao.Execute(sql, model);
						}
					}
					scope.Complete();
				}
				return true;
			}
			catch(Exception ex)
			{
				return false;
			}
		}

		public List<Traducao> Sublinha(Guid idSublinha)
        {
			var retorno = new List<Traducao>();

			using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
				#region Consulta
				retorno = conexao.Query<Traducao>(
				@"
					SELECT * FROM (
                    SELECT 
						'NomeSubLinha' as Campo,
						'SubLinhaIdioma' as Tabela,
						'IdSubLinha' as CampoRef,
						SubLinha.Id, SubLinha.NomeSubLinha AS PT,
						SubLinhaEN.Id as IdEN, SubLinhaEN.NomeSubLinha as EN,
						SubLinhaES.Id as IdES, SubLinhaES.NomeSubLinha as ES
					FROM
						SubLinha
					LEFT JOIN
						SubLinhaIdioma SubLinhaEN
					ON 
						SubLinha.Id = SubLinhaEN.IdSubLinha AND
						SubLinhaEN.CodigoIdioma = 2
					LEFT JOIN
						SubLinhaIdioma SubLinhaES
					ON 
						SubLinha.Id = SubLinhaES.IdSubLinha AND
						SubLinhaES.CodigoIdioma = 3
					WHERE
						Sublinha.Id = @IdSubLinha

					UNION

					SELECT 
						'TextoBotao' as Campo,
						'SubLinhaIdioma' as Tabela,
						'IdSubLinha' as CampoRef,
						SubLinha.Id, SubLinha.TextoBotao AS PT,
						SubLinhaEN.Id as IdEN, SubLinhaEN.TextoBotao as EN,
						SubLinhaES.Id as IDES, SubLinhaES.TextoBotao as ES
					FROM
						SubLinha
					LEFT JOIN
						SubLinhaIdioma SubLinhaEN
					ON 
						SubLinha.Id = SubLinhaEN.IdSubLinha AND
						SubLinhaEN.CodigoIdioma = 2
					LEFT JOIN
						SubLinhaIdioma SubLinhaES
					ON 
						SubLinha.Id = SubLinhaES.IdSubLinha AND
						SubLinhaES.CodigoIdioma = 3
					WHERE
						Sublinha.Id = @IdSubLinha

					UNION

					SELECT 
						'TextoInformativo' as CAMPO,
						'SubLinhaIdioma' as Tabela,
						'IdSubLinha' as CampoRef,
						SubLinha.Id, SubLinha.TextoInformativo AS PT,
						SubLinhaEN.Id as IdEN, SubLinhaEN.TextoInformativo as EN,
						SubLinhaES.Id as IdES, SubLinhaES.TextoInformativo as ES
					FROM
						SubLinha
					LEFT JOIN
						SubLinhaIdioma SubLinhaEN
					ON 
						SubLinha.Id = SubLinhaEN.IdSubLinha AND
						SubLinhaEN.CodigoIdioma = 2
					LEFT JOIN
						SubLinhaIdioma SubLinhaES
					ON 
						SubLinha.Id = SubLinhaES.IdSubLinha AND
						SubLinhaES.CodigoIdioma = 3
					WHERE
						Sublinha.Id = @IdSubLinha

					UNION

					SELECT 
						'NomeClassificacao' as CAMPO,
						'ClassificacaoIdioma' as Tabela,
						'IdClassificacao' as CampoRef,
						Classificacao.Id, Classificacao.NomeClassificacao AS PT,
						ClassificacaoIdiomaEN.Id as IdEN, ClassificacaoIdiomaEN.NomeClassificacao as EN,
						ClassificacaoIdiomaES.Id as IdES, ClassificacaoIdiomaES.NomeClassificacao as ES
					FROM
						Classificacao
					LEFT JOIN
						ClassificacaoIdioma as ClassificacaoIdiomaEN
					ON
						Classificacao.Id = ClassificacaoIdiomaEN.IdClassificacao AND
						ClassificacaoIdiomaEN.CodigoIdioma = 2
					LEFT JOIN
						ClassificacaoIdioma as ClassificacaoIdiomaES
					ON
						Classificacao.Id = ClassificacaoIdiomaES.IdClassificacao AND
						ClassificacaoIdiomaES.CodigoIdioma = 3
					WHERE
						Classificacao.IdSubLinha = @IdSubLinha

					UNION

					SELECT 
						'NomeEspecificacao' as CAMPO,
						'EspecificacaoTecnicaIdioma' as Tabela,
						'IdEspecificacaoTecnica' as CampoRef,
						EspecificacaoTecnica.Id,	     EspecificacaoTecnica.NomeEspecificacao AS PT,
						EspecificacaoTecnicaIdiomaEN.Id as IdEN, EspecificacaoTecnicaIdiomaEN.NomeEspecificacao as EN,
						EspecificacaoTecnicaIdiomaES.Id as IdES, EspecificacaoTecnicaIdiomaES.NomeEspecificacao as ES
					FROM
						EspecificacaoTecnica
					LEFT JOIN
						EspecificacaoTecnicaIdioma as EspecificacaoTecnicaIdiomaEN
					ON
						EspecificacaoTecnica.Id = EspecificacaoTecnicaIdiomaEN.IdEspecificacaoTecnica AND
						EspecificacaoTecnicaIdiomaEN.CodigoIdioma = 2
					LEFT JOIN
						EspecificacaoTecnicaIdioma as EspecificacaoTecnicaIdiomaES
					ON
						EspecificacaoTecnica.Id = EspecificacaoTecnicaIdiomaES.IdEspecificacaoTecnica AND
						EspecificacaoTecnicaIdiomaES.CodigoIdioma = 3
					WHERE
						EspecificacaoTecnica.IdSubLinha = @IdSubLinha
				) A
				WHERE
					PT IS NOT NULL AND PT <> ''
                ", new { idSublinha }).ToList();
				#endregion
			}

			retorno = Ordenar(retorno, new List<string> { "NomeSubLinha", "TextoBotao", "TextoInformativo", "NomeClassificacao", "NomeEspecificacao" });

			return retorno;
		}

		public List<Traducao> Produto(Guid idProduto)
		{
			var retorno = new List<Traducao>();

			using (SqlConnection conexao = new SqlConnection(ConnectionString))
			{
				#region Consulta
				retorno = conexao.Query<Traducao>(
				@"
                    SELECT * FROM (
						SELECT 
							'NomeProduto' as Campo,
							'ProdutoIdioma' as Tabela,
							'IdProduto' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.NomeProduto AS PT,
							ItemEN.Id as IdEN, ItemEN.NomeProduto as EN,
							ItemES.Id as IdES, ItemES.NomeProduto as ES
						FROM
							Produto ItemPT
						LEFT JOIN
							ProdutoIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdProduto AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							ProdutoIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdProduto AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.Id = @IdProduto

						UNION

						SELECT 
							'Texto1' as Campo,
							'SecaoProdutoIdioma' as Tabela,
							'IdSecaoProduto' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.Texto1 AS PT,
							ItemEN.Id as IdEN, ItemEN.Texto1 as EN,
							ItemES.Id as IdES, ItemES.Texto1 as ES
						FROM
							SecaoProduto ItemPT
						LEFT JOIN
							SecaoProdutoIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdSecaoProduto AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							SecaoProdutoIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdSecaoProduto AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdProduto = @IdProduto

						UNION

						SELECT 
							'Texto2' as Campo,
							'SecaoProdutoIdioma' as Tabela,
							'IdSecaoProduto' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.Texto2 AS PT,
							ItemEN.Id as IdEN, ItemEN.Texto2 as EN,
							ItemES.Id as IdES, ItemES.Texto2 as ES
						FROM
							SecaoProduto ItemPT
						LEFT JOIN
							SecaoProdutoIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdSecaoProduto AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							SecaoProdutoIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdSecaoProduto AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdProduto = @IdProduto

						UNION

						SELECT 
							'Texto3' as Campo,
							'SecaoProdutoIdioma' as Tabela,
							'IdSecaoProduto' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.Texto3 AS PT,
							ItemEN.Id as IdEN, ItemEN.Texto3 as EN,
							ItemES.Id as IdES, ItemES.Texto3 as ES
						FROM
							SecaoProduto ItemPT
						LEFT JOIN
							SecaoProdutoIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdSecaoProduto AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							SecaoProdutoIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdSecaoProduto AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdProduto = @IdProduto

						UNION

						SELECT 
							'CodigoVideo' as Campo,
							'SecaoProdutoIdioma' as Tabela,
							'IdSecaoProduto' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.CodigoVideo AS PT,
							ItemEN.Id as IdEN, ItemEN.CodigoVideo as EN,
							ItemES.Id as IdES, ItemES.CodigoVideo as ES
						FROM
							SecaoProduto ItemPT
						LEFT JOIN
							SecaoProdutoIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdSecaoProduto AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							SecaoProdutoIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdSecaoProduto AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdProduto = @IdProduto

						UNION

						SELECT 
							'CodigoVideo2' as Campo,
							'SecaoProdutoIdioma' as Tabela,
							'IdSecaoProduto' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.CodigoVideo2 AS PT,
							ItemEN.Id as IdEN, ItemEN.CodigoVideo2 as EN,
							ItemES.Id as IdES, ItemES.CodigoVideo2 as ES
						FROM
							SecaoProduto ItemPT
						LEFT JOIN
							SecaoProdutoIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdSecaoProduto AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							SecaoProdutoIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdSecaoProduto AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdProduto = @IdProduto


						UNION

						SELECT 
							'CodigoVideo3' as Campo,
							'SecaoProdutoIdioma' as Tabela,
							'IdSecaoProduto' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.CodigoVideo3 AS PT,
							ItemEN.Id as IdEN, ItemEN.CodigoVideo3 as EN,
							ItemES.Id as IdES, ItemES.CodigoVideo3 as ES
						FROM
							SecaoProduto ItemPT
						LEFT JOIN
							SecaoProdutoIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdSecaoProduto AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							SecaoProdutoIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdSecaoProduto AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdProduto = @IdProduto

						UNION

						SELECT 
							'CodigoVideo4' as Campo,
							'SecaoProdutoIdioma' as Tabela,
							'IdSecaoProduto' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.CodigoVideo4 AS PT,
							ItemEN.Id as IdEN, ItemEN.CodigoVideo4 as EN,
							ItemES.Id as IdES, ItemES.CodigoVideo4 as ES
						FROM
							SecaoProduto ItemPT
						LEFT JOIN
							SecaoProdutoIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdSecaoProduto AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							SecaoProdutoIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdSecaoProduto AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdProduto = @IdProduto

						UNION

						SELECT 
							'DescricaoIcone' as Campo,
							'SecaoProdutoIconeIdioma' as Tabela,
							'IdSecaoProdutoIcone' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.DescricaoIcone AS PT,
							ItemEN.Id as IdEN, ItemEN.DescricaoIcone as EN,
							ItemES.Id as IdES, ItemES.DescricaoIcone as ES
						FROM
							SecaoProdutoIcone ItemPT
						LEFT JOIN
							SecaoProdutoIconeIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdSecaoProdutoIcone AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							SecaoProdutoIconeIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdSecaoProdutoIcone AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdSecaoProduto IN (SELECT ID FROM SecaoProduto WHERE idProduto = @IdProduto)

						UNION

						SELECT 
							'SubDescricaoIcone' as Campo,
							'SecaoProdutoIconeIdioma' as Tabela,
							'IdSecaoProdutoIcone' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.SubDescricaoIcone AS PT,
							ItemEN.Id as IdEN, ItemEN.SubDescricaoIcone as EN,
							ItemES.Id as IdES, ItemES.SubDescricaoIcone as ES
						FROM
							SecaoProdutoIcone ItemPT
						LEFT JOIN
							SecaoProdutoIconeIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdSecaoProdutoIcone AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							SecaoProdutoIconeIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdSecaoProdutoIcone AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdSecaoProduto IN (SELECT ID FROM SecaoProduto WHERE idProduto = @IdProduto)

						UNION

						SELECT 
							'NomeArquivo' as Campo,
							'ArquivoIdioma' as Tabela,
							'IdArquivo' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.NomeArquivo AS PT,
							ItemEN.Id as IdEN, ItemEN.NomeArquivo as EN,
							ItemES.Id as IdES, ItemES.NomeArquivo as ES
						FROM
							Arquivo ItemPT
						LEFT JOIN
							ArquivoIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdArquivo AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							ArquivoIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdArquivo AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdPai = @IdProduto

						UNION

						SELECT 
							'Valor' as Campo,
							'ProdutoEspecificacaoTecnicaIdioma' as Tabela,
							'IdProdutoEspecificacaoTecnica' as CampoRef,
							CAST(ItemPT.IdProduto AS VARCHAR(200)) + CAST(ItemPT.IdEspecificacaoTecnica AS VARCHAR(200)) as Id, ItemPT.Valor AS PT,
							ItemEN.IdProdutoEspecificacaoTecnica as IdEN, ItemEN.Valor as EN,
							ItemES.IdProdutoEspecificacaoTecnica as IdES, ItemES.Valor as ES
						FROM
							ProdutoEspecificacaoTecnica ItemPT
						LEFT JOIN
							ProdutoEspecificacaoTecnicaIdioma ItemEN
						ON 
							CAST(ItemPT.IdProduto AS VARCHAR(200)) + CAST(ItemPT.IdEspecificacaoTecnica AS VARCHAR(200)) = ItemEN.IdProdutoEspecificacaoTecnica AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							ProdutoEspecificacaoTecnicaIdioma ItemES
						ON 
							CAST(ItemPT.IdProduto AS VARCHAR(200)) + CAST(ItemPT.IdEspecificacaoTecnica AS VARCHAR(200)) = ItemES.IdProdutoEspecificacaoTecnica AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdProduto = @IdProduto

						UNION

						SELECT 
							'DescricaoCaracteristica' as Campo,
							'CaracteristicaIdioma' as Tabela,
							'IdCaracteristica' as CampoRef,
							CAST(ItemPT.Id AS VARCHAR(200)) AS ID, ItemPT.DescricaoCaracteristica AS PT,
							ItemEN.Id as IdEN, ItemEN.DescricaoCaracteristica as EN,
							ItemES.Id as IdES, ItemES.DescricaoCaracteristica as ES
						FROM
							Caracteristica ItemPT
						LEFT JOIN
							CaracteristicaIdioma ItemEN
						ON 
							ItemPT.Id = ItemEN.IdCaracteristica AND
							ItemEN.CodigoIdioma = 2
						LEFT JOIN
							CaracteristicaIdioma ItemES
						ON 
							ItemPT.Id = ItemES.IdCaracteristica AND
							ItemES.CodigoIdioma = 3
						WHERE
							ItemPT.IdProduto = @IdProduto
						) A
						WHERE
							PT IS NOT NULL AND PT <> ''
                ", new { idProduto }).ToList();
				#endregion
			}

			retorno = Ordenar(retorno, new List<string> {	"NomeProduto", "Texto1", "Texto2", "Texto3", 
															"CodigoVideo", "CodigoVideo2", "CodigoVideo3", "CodigoVideo4", 
															"DescricaoIcone", "SubDescricaoIcone", "NomeArquivo", "Valor",
															"DescricaoCaracteristica", });

			return retorno;
		}

		public List<Traducao> Banner(string codigoBanner)
		{
			var retorno = new List<Traducao>();

			using (SqlConnection conexao = new SqlConnection(ConnectionString))
			{
				#region Consulta
				retorno = conexao.Query<Traducao>(
				@"
					SELECT * FROM (
                    SELECT 
						'texto1' as Campo,
						'BannerIdioma' as Tabela,
						'IdBanner' as CampoRef,
						ItemPT.Id, ItemPT.texto1 AS PT,
						ItemEN.Id as IdEN, ItemEN.texto1 as EN,
						ItemES.Id as IdES, ItemES.texto1 as ES
					FROM
						Banner ItemPT
					LEFT JOIN
						BannerIdioma ItemEN
					ON 
						ItemPT.Id = ItemEN.IdBanner AND
						ItemEN.CodigoIdioma = 2
					LEFT JOIN
						BannerIdioma ItemES
					ON 
						ItemPT.Id = ItemES.IdBanner AND
						ItemES.CodigoIdioma = 3
					WHERE
						ItemPT.Modulo = @codigoBanner

					UNION

					SELECT 
						'texto2' as Campo,
						'BannerIdioma' as Tabela,
						'IdBanner' as CampoRef,
						ItemPT.Id, ItemPT.texto2 AS PT,
						ItemEN.Id as IdEN, ItemEN.texto2 as EN,
						ItemES.Id as IdES, ItemES.texto2 as ES
					FROM
						Banner ItemPT
					LEFT JOIN
						BannerIdioma ItemEN
					ON 
						ItemPT.Id = ItemEN.IdBanner AND
						ItemEN.CodigoIdioma = 2
					LEFT JOIN
						BannerIdioma ItemES
					ON 
						ItemPT.Id = ItemES.IdBanner AND
						ItemES.CodigoIdioma = 3
					WHERE
						ItemPT.Modulo = @codigoBanner

					UNION

					SELECT 
						'texto3' as Campo,
						'BannerIdioma' as Tabela,
						'IdBanner' as CampoRef,
						ItemPT.Id, ItemPT.texto3 AS PT,
						ItemEN.Id as IdEN, ItemEN.texto3 as EN,
						ItemES.Id as IdES, ItemES.texto3 as ES
					FROM
						Banner ItemPT
					LEFT JOIN
						BannerIdioma ItemEN
					ON 
						ItemPT.Id = ItemEN.IdBanner AND
						ItemEN.CodigoIdioma = 2
					LEFT JOIN
						BannerIdioma ItemES
					ON 
						ItemPT.Id = ItemES.IdBanner AND
						ItemES.CodigoIdioma = 3
					WHERE
						ItemPT.Modulo = @codigoBanner

				) A
				WHERE
					PT IS NOT NULL AND PT <> ''
                ", new { codigoBanner }).ToList();
				#endregion
			}

			retorno = Ordenar(retorno, new List<string> { "Texto1", "Texto2", "Texto3" });

			return retorno;
		}

		public List<Traducao> Refrigeracao()
		{
			var retorno = new List<Traducao>();

			using (SqlConnection conexao = new SqlConnection(ConnectionString))
			{
				#region Consulta
				retorno = conexao.Query<Traducao>(
				@"
					SELECT * FROM (
                    SELECT 
						'TituloVideo' as Campo,
						'VideoIdioma' as Tabela,
						'IdVideo' as CampoRef,
						ItemPT.Id, ItemPT.TituloVideo AS PT,
						ItemEN.Id as IdEN, ItemEN.TituloVideo as EN,
						ItemES.Id as IdES, ItemES.TituloVideo as ES
					FROM
						Video ItemPT
					LEFT JOIN
						VideoIdioma ItemEN
					ON 
						ItemPT.Id = ItemEN.IdVideo AND
						ItemEN.CodigoIdioma = 2
					LEFT JOIN
						VideoIdioma ItemES
					ON 
						ItemPT.Id = ItemES.IdVideo AND
						ItemES.CodigoIdioma = 3
					WHERE
						ItemPT.Modulo LIKE 'Refrigeracao%'

					UNION

					SELECT 
						'DescricaoVideo' as Campo,
						'VideoIdioma' as Tabela,
						'IdVideo' as CampoRef,
						ItemPT.Id, ItemPT.DescricaoVideo AS PT,
						ItemEN.Id as IdEN, ItemEN.DescricaoVideo as EN,
						ItemES.Id as IdES, ItemES.DescricaoVideo as ES
					FROM
						Video ItemPT
					LEFT JOIN
						VideoIdioma ItemEN
					ON 
						ItemPT.Id = ItemEN.IdVideo AND
						ItemEN.CodigoIdioma = 2
					LEFT JOIN
						VideoIdioma ItemES
					ON 
						ItemPT.Id = ItemES.IdVideo AND
						ItemES.CodigoIdioma = 3
					WHERE
						ItemPT.Modulo LIKE 'Refrigeracao%'

					UNION

					SELECT 
						'Titulo' as Campo,
						'EventoRefrigeracaoIdioma' as Tabela,
						'IdEventoRefrigeracao' as CampoRef,
						ItemPT.Id, ItemPT.Titulo AS PT,
						ItemEN.Id as IdEN, ItemEN.Titulo as EN,
						ItemES.Id as IdES, ItemES.Titulo as ES
					FROM
						EventoRefrigeracao ItemPT
					LEFT JOIN
						EventoRefrigeracaoIdioma ItemEN
					ON 
						ItemPT.Id = ItemEN.IdEventoRefrigeracao AND
						ItemEN.CodigoIdioma = 2
					LEFT JOIN
						EventoRefrigeracaoIdioma ItemES
					ON 
						ItemPT.Id = ItemES.IdEventoRefrigeracao AND
						ItemES.CodigoIdioma = 3

				) A
				WHERE
					PT IS NOT NULL AND PT <> ''
                ", new {  }).ToList();
				#endregion
			}

			retorno = Ordenar(retorno, new List<string> {   "TituloVideo", "DescricaoVideo", "Titulo" });

			return retorno;
		}

		private List<Traducao> Ordenar(List<Traducao> list, List<string> ordemCampos)
		{
			var retorno = new List<Traducao>();
			foreach (var ordem in ordemCampos)
			{
				retorno.AddRange(list.Where(x => x.campo.ToUpper() == ordem.ToUpper()));
			}

			return retorno;
		}

	}
}
