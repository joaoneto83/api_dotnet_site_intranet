using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.IO;

namespace Elgin.Portal.Services.Implementation
{
    public class SolarIntegradorService : AbstractService<SolarIntegradorService>
    {

        public SolarIntegradorService(string connectionString) : base(connectionString) { }

        public List<SolarIntegrador> Listar()
        {
            var retorno = new List<SolarIntegrador>();

            var sql = @"
                  SELECT SolarIntegrador.Id
                        ,UF
                        ,Nome
                        ,CNPJ
                        ,Endereco
                        ,Email
                        ,Telefone
                        ,Site
                        ,IdCidade
                        ,SolarIntegrador.Ativo
	                    ,Cidade.Descricao AS NomeCidade
                    FROM SolarIntegrador
                    LEFT JOIN Cidade ON
                    Cidade.Id = SolarIntegrador.IdCidade
                    WHERE SolarIntegrador.Ativo = 1
                    ORDER BY Nome
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<SolarIntegrador>(sql).Distinct().ToList();
            }

            return retorno;
        }
        public SolarIntegrador ProcuraIntegradorPorId(Guid idIntegrador)
        {
            var retorno = new SolarIntegrador();

            var sql = @"
                  SELECT Id
                        ,UF
                        ,Nome
                        ,CNPJ
                        ,Endereco
                        ,Email
                        ,Telefone
                        ,Site
                        ,IdCidade
                        ,Ativo
                    FROM SolarIntegrador
                    WHERE Id = @idIntegrador
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<SolarIntegrador>(sql, new { idIntegrador }).FirstOrDefault();
            }

            return retorno;
        }

        public bool Salvar(SolarIntegrador model)
        {
            try
            {
                model.CNPJ = model.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");
                if (model.Id != Guid.Empty)
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             UPDATE [SolarIntegrador]
                                SET [UF] = @UF
                                    ,[Nome] = @Nome
                                    ,[CNPJ] = @CNPJ
                                    ,[Endereco] = @Endereco
                                    ,[Email] = @Email
                                    ,[Telefone] = @Telefone
                                    ,[Site] = @Site
                                    ,[IdCidade] = @IdCidade
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
                             INSERT INTO [SolarIntegrador]
                                        ([Id]
                                        ,[UF]
                                        ,[Nome]
                                        ,[CNPJ]
                                        ,[Endereco]
                                        ,[Email]
                                        ,[Telefone]
                                        ,[Site]
                                        ,[IdCidade]
                                        ,[Ativo])
                                    VALUES
                                        (@Id
                                        ,@UF
                                        ,@Nome
                                        ,@CNPJ
                                        ,@Endereco
                                        ,@Email
                                        ,@Telefone
                                        ,@Site
                                        ,@IdCidade
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
    }
}
