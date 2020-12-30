using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;

namespace Elgin.Portal.Services.Implementation
{
    public class ClassificacaoService : AbstractService<ClassificacaoService>
    {
        public ClassificacaoService(string connectionString) : base(connectionString) { }

        public List<Classificacao> ListarClassificacaoPorSubLinha(string codigoSubLinha, int idioma)
        {
            var retorno = new List<Classificacao>();

            bool possuiPilha;

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                possuiPilha = conexao.Query<bool>(
                @"
                    SELECT PossuiFiltroPilha
                      FROM [SubLinha]
                      WHERE CodigoSubLinha = @codigoSubLinha
                ", new { codigoSubLinha }).FirstOrDefault();
            }

            var sql = @"SELECT
                            Classificacao.[Id]
                           ,[NomeClassificacao]
                            ,[IdSubLinha]
                            ,Classificacao.[Ativo]
                            ,[IdClassificacaoSuperior]
                            ,Classificacao.[CaminhoImagem]
                            ,Classificacao.Comparativo
                            ,Classificacao.Ordem
                            ,Classificacao.Expansivo
                            ,Classificacao.PossuiFiltroPilha
                        FROM [Classificacao]
                        LEFT JOIN SubLinha 
                            ON SubLinha.Id = Classificacao.IdSubLinha
                        WHERE SubLinha.CodigoSubLinha = @codigoSubLinha 
                            AND Classificacao.Ativo = 1";

            if (possuiPilha)
            {
                sql += @"OR (NomeClassificacao = 'Pilhas' 
                            OR IdClassificacaoSuperior = (SELECT Id FROM Classificacao WHERE NomeClassificacao = 'Pilhas'))
                        ORDER BY CASE WHEN Classificacao.OrdemImagem IS NOT NULL THEN Classificacao.OrdemImagem END";
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Classificacao>(sql, new { codigoSubLinha, idioma }).ToList();
            }

            foreach (var item in retorno)
            {
                item.Filhos.AddRange(retorno.Where(x => x.IdClassificacaoSuperior == item.Id).ToList());
            }

            return retorno.Where(x => x.IdClassificacaoSuperior == null).ToList();
        }

        public List<Classificacao> ListarClassificacaoPorComparativo(string codigoSubLinha, int idioma)
        {
            var retorno = new List<Classificacao>();

            bool possuiPilha;

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                possuiPilha = conexao.Query<bool>(
                @"
                    SELECT PossuiFiltroPilha
                      FROM [SubLinha]
                      WHERE CodigoSubLinha = @codigoSubLinha
                ", new { codigoSubLinha }).FirstOrDefault();
            }

            var sql = @"SELECT
                            Classificacao.[Id]
                            ,[NomeClassificacao]
                            ,[IdSubLinha]
                            ,Classificacao.[Ativo]
                            ,[IdClassificacaoSuperior]
                            ,Classificacao.[CaminhoImagem]
                            ,Classificacao.Comparativo
                            ,Classificacao.Ordem
                            ,Classificacao.Expansivo
                            ,Classificacao.PossuiFiltroPilha
                        FROM [Classificacao]
                        LEFT JOIN SubLinha ON SubLinha.Id = Classificacao.IdSubLinha
                        WHERE SubLinha.CodigoSubLinha = @codigoSubLinha 
                            AND Classificacao.Ativo = 1 
                            AND (Classificacao.Comparativo = 1 OR IdClassificacaoSuperior IS NULL)";

            if (possuiPilha)
            {
                sql += @"OR (NomeClassificacao = 'Pilhas' 
                            OR IdClassificacaoSuperior = (SELECT Id FROM Classificacao WHERE NomeClassificacao = 'Pilhas'))
                        ORDER BY CASE WHEN Classificacao.OrdemImagem IS NOT NULL THEN Classificacao.OrdemImagem END";
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Classificacao>(sql, new { codigoSubLinha, idioma }).ToList();
            }

            foreach (var item in retorno)
            {
                item.Filhos.AddRange(retorno.Where(x => x.IdClassificacaoSuperior == item.Id).ToList());
            }

            return retorno.Where(x => x.IdClassificacaoSuperior == null).ToList();
        }
    }
}
