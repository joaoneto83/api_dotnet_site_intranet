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
    public class LinhaService : AbstractService<LinhaService>
    {
        public LinhaService(string connectionString) : base(connectionString) { }

        public Linha ProcurarPorId(Guid id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Linha>(
                @"
                    SELECT  
                        Id,
		                CodigoLinha,
		                NomeLinha,
		                Cor1,
		                Cor2,
		                CorTitulo,
                        Ordem
                    FROM Linha
                    WHERE Id = @id
                ", new { id }).FirstOrDefault();
            }
        }

        public List<Linha> ListarLinhas(int codigoIdioma = 1)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Linha>(
                @"
                    SELECT  
                        Linha.Id,
	                    CASE WHEN LinhaIdioma.NomeLinha IS NULL THEN
		                    Linha.NomeLinha
	                    ELSE
		                    LinhaIdioma.NomeLinha
	                    END as NomeLinha,
	                    Linha.CodigoLinha,
	                    Linha.Cor1,
	                    Linha.Cor2,
	                    Linha.CorTitulo,
                        Linha.Ordem	
                    FROM 
	                    Linha
                    LEFT JOIN
	                    LinhaIdioma
                    ON
	                    Linha.Id = LinhaIdioma.IdLinha AND
	                    LinhaIdioma.CodigoIdioma = @codigoIdioma
                    WHERE 
	                    Linha.Ativo = 1
                    ORDER BY 
	                    Linha.Ordem
                ", new { codigoIdioma }).ToList();
            }
        }

        public List<Linha> ListarLinhasComSublinha(bool possuiMenusEspeciais)
        {
            var retorno = new List<Linha>();
            var whereMenusEspeciais = string.Empty;

            if (!possuiMenusEspeciais)
                whereMenusEspeciais = "AND Linha.CodigoLinha != 'Automacao' AND Linha.CodigoLinha != 'MaquinaSorvete' AND Linha.CodigoLinha != 'Refrigeracao'";

            var sql = $@"
                       SELECT  
                            Linha.Id,
	                        Linha.CodigoLinha,
	                        Linha.NomeLinha,
	                        Linha.Cor1,
	                        Linha.Cor2,
	                        Linha.CorTitulo,
                            Linha.Ordem,
	                        SubLinha.NomeSubLinha,
	                        SubLinha.CodigoSubLinha,
                            SubLinha.Id,
                            SubLinha.Ordem,
	                        Arquivo.Caminho AS CaminhoImagem
                        FROM Linha
                        LEFT JOIN SubLinha
                        ON Linha.Id = SubLinha.IdLinha AND SubLinha.Ativo = 1 {whereMenusEspeciais}
                        LEFT JOIN Arquivo
                        ON Arquivo.Id = Sublinha.IdArquivo
                        WHERE Linha.Ativo = 1
                        ORDER BY Linha.Ordem
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, Linha>();

                retorno = conexao.Query<Linha, SubLinha, Linha>(sql,
                (linha, sublinha) =>
                {
                    Linha _linha;

                    if (!dictionary.TryGetValue(linha.Id, out _linha))
                    {
                        _linha = linha;
                        _linha.SubLinhas = new List<SubLinha>();
                        dictionary.Add(_linha.Id, _linha);
                    }

                    if (sublinha != null)
                        _linha.SubLinhas.Add(sublinha);

                    return _linha;
                }, new {  },
                splitOn: "NomeSubLinha").Distinct().ToList();
            }

            return retorno;
        }

        public List<Linha> ListarLinhasComTodasSublinhas()
        {
            var retorno = new List<Linha>();

            var sql = $@"
                       SELECT  
                            Linha.Id,
	                        Linha.CodigoLinha,
	                        Linha.NomeLinha,
	                        Linha.Cor1,
	                        Linha.Cor2,
	                        Linha.CorTitulo,
                            Linha.Ordem,
	                        SubLinha.NomeSubLinha,
	                        SubLinha.CodigoSubLinha,
                            SubLinha.Id,
                            SubLinha.Ordem,
	                        Arquivo.Caminho AS CaminhoImagem
                        FROM Linha
                        LEFT JOIN SubLinha
                        ON Linha.Id = SubLinha.IdLinha
                        LEFT JOIN Arquivo
                        ON Arquivo.Id = Sublinha.IdArquivo
                        WHERE Linha.Ativo = 1
                        ORDER BY Linha.Ordem
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, Linha>();

                retorno = conexao.Query<Linha, SubLinha, Linha>(sql,
                (linha, sublinha) =>
                {
                    Linha _linha;

                    if (!dictionary.TryGetValue(linha.Id, out _linha))
                    {
                        _linha = linha;
                        _linha.SubLinhas = new List<SubLinha>();
                        dictionary.Add(_linha.Id, _linha);
                    }

                    if (sublinha != null)
                        _linha.SubLinhas.Add(sublinha);

                    return _linha;
                }, new { },
                splitOn: "NomeSubLinha").Distinct().ToList();
            }

            return retorno;
        }
    }
}
