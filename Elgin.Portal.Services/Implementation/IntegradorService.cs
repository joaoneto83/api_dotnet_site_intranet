using Dapper;
using Elgin.Portal.Services.Implementation.Shared;
using Elgin.Portal.Services.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Elgin.Portal.Services.Implementation
{
    public class IntegradorService : AbstractService<IntegradorService>
    {
        public IntegradorService(string connectionString) : base(connectionString) { }

        public List<Integrador> Listar()
        {
            var retorno = new List<Integrador>();

            var sql = @"
                  SELECT Integrador.Id
                        ,Integrador.Nome
                        ,Endereco
                        ,TelefoneFixo
                        ,TelefoneMovel
                        ,Email
                        ,Site
                        ,IdSegmento
                        ,IdPais
                        ,Uf
                        ,IdCidade
                        ,Integrador.Ativo
                        ,Pais.Nome AS NomePais
	                    ,Cidade.Descricao AS NomeCidade
                        ,Segmento.Nome AS NomeSegmento
                    FROM Integrador
                    LEFT JOIN Cidade ON Cidade.Id = Integrador.IdCidade
                    LEFT JOIN Pais ON Pais.Id = Integrador.IdPais
                    LEFT JOIN Segmento ON Segmento.Id = Integrador.IdSegmento
                    WHERE Integrador.Ativo = 1
                    ORDER BY Integrador.Nome
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Integrador>(sql).Distinct().ToList();
            }

            return retorno;
        }
        public Integrador ProcuraIntegradorPorId(Guid idIntegrador)
        {
            var retorno = new Integrador();

            var sql = @"
                  SELECT Id
                        ,Nome
                        ,Endereco
                        ,Email
                        ,TelefoneFixo
                        ,TelefoneMovel
                        ,Site
                        ,IdPais
                        ,Uf
                        ,IdCidade
                        ,IdSegmento
                        ,Ativo
                    FROM Integrador
                    WHERE Id = @idIntegrador
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Integrador>(sql, new { idIntegrador }).FirstOrDefault();
            }

            return retorno;
        }

        public bool Salvar(Integrador model)
        {
            try
            {
                if (model.Id != Guid.Empty)
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             UPDATE [Integrador]
                                SET [Nome] = @Nome
                                    ,[Endereco] = @Endereco
                                    ,[Email] = @Email
                                    ,[TelefoneFixo] = @TelefoneFixo
                                    ,[TelefoneMovel] = @TelefoneMovel
                                    ,[Site] = @Site
                                    ,[IdCidade] = @IdCidade
                                    ,[Uf] = @Uf
                                    ,[IdPais] = @IdPais
                                    ,[IdSegmento] = @IdSegmento
                                    ,[Ativo] = @Ativo
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
                             INSERT INTO [Integrador]
                                        ([Id]
                                        ,[Nome]
                                        ,[Endereco]
                                        ,[Email]
                                        ,[TelefoneFixo]
                                        ,[TelefoneMovel]
                                        ,[Site]
                                        ,[IdCidade]
                                        ,[Uf]
                                        ,[IdPais]
                                        ,[IdSegmento]
                                        ,[Ativo])
                                    VALUES
                                        (@Id
                                        ,@Nome
                                        ,@Endereco
                                        ,@Email
                                        ,@TelefoneFixo
                                        ,@TelefoneMovel
                                        ,@Site
                                        ,@IdCidade
                                        ,@Uf
                                        ,@IdPais
                                        ,@IdSegmento
                                        ,@Ativo)
                        ", model);
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public List<Integrador> FiltrarIntegradores(int? idPais, string uf, string segmento)
        {
            var retorno = new List<Integrador>();

            var sql = @"
                  SELECT Integrador.Id
                        ,Integrador.Nome
                        ,Integrador.Email
                        ,Endereco
                        ,TelefoneFixo
                        ,TelefoneMovel
                        ,IdSegmento
                        ,IdPais
                        ,Uf
                        ,IdCidade
                        ,Integrador.Ativo
                        ,Pais.Nome AS NomePais
	                    ,Cidade.Descricao AS NomeCidade
                        ,Segmento.Nome AS NomeSegmento
                    FROM Integrador
                    LEFT JOIN Cidade ON Cidade.Id = Integrador.IdCidade
                    LEFT JOIN Pais ON Pais.Id = Integrador.IdPais
                    LEFT JOIN Segmento ON Segmento.Id = Integrador.IdSegmento
                    WHERE Integrador.Ativo = 1
                          AND (@idPais IS NULL OR Integrador.IdPais = @idPais)
                          AND (@uf IS NULL OR Integrador.UF = @uf)
                          AND (@segmento IS NULL OR Segmento.Codigo = @segmento)
                    ORDER BY Integrador.Nome
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Integrador>(sql, new { idPais, uf, segmento }).Distinct().ToList();
            }

            return retorno;
        }
    }
}
