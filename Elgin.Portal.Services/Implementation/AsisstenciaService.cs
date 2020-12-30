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
    public class AssistenciaService : AbstractService<AssistenciaService>
    {
        public AssistenciaService(string connectionString) : base(connectionString) { }

        public List<Assistencia> Listar(string uf, int idCidade)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Assistencia>(
                @"
                        SELECT Assistencia.Id
                              ,Assistencia.Nome
                              ,Assistencia.Rua
                              ,Assistencia.Numero
                              ,Assistencia.Bairro
                              ,Assistencia.Complemento
                              ,Assistencia.Cep
                              ,Assistencia.Telefone
                              ,Assistencia.Telefone_2
                              ,Assistencia.Telefone_3
                              ,Assistencia.Telefone_4
                              ,Assistencia.Email
                              ,Assistencia.IdCidade
                              ,Assistencia.IdEstado
                              ,Assistencia.Lat
                              ,Assistencia.Long
                              ,Assistencia.Ativo
                              ,Estado.Descricao as NomeEstado
                              ,Cidade.Descricao as NomeCidade
                        FROM 
                            Assistencia 
                        JOIN
                            Cidade
                        ON
                            Cidade.Id = Assistencia.IdCidade
                        JOIN
                            Estado
                        ON
                            Estado.Id = Assistencia.IdEstado
                        WHERE
                            Estado.Sigla = @uf AND
                            Cidade.Id = @idCidade
    
                ", new { uf, idCidade }).ToList();
            }
        }

        public List<Cidade> ListaCidades(string estado)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Cidade>(
                    @"
                    SELECT C.[Id], C.[Descricao]
                    FROM [dbo].[Cidade] C
                    INNER JOIN [dbo].[Estado] E ON E.Id = C.IdEstado AND E.Id = @estado
                    ORDER BY C.[Descricao]

                ", new { estado }).ToList();
            }
        }

        public List<Cidade> BuscarCidade(string uf)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Cidade>(
                @"
                        SELECT Cidade.Id
                              ,Cidade.Descricao
                              ,IdEstado
                              ,EstadoUf
                        FROM 
                            Cidade 
                  
                        WHERE
                            
                             Cidade.IdEstado = @uf 
                          
    
                ", new { uf }).ToList();
            }

        }

        public Assistencia GetAssistencia(Guid id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Assistencia>(
             @"
                        SELECT Assistencia.Id
                              ,Assistencia.Nome
                              ,Assistencia.Rua
                              ,Assistencia.Numero
                              ,Assistencia.Bairro
                              ,Assistencia.Complemento
                              ,Assistencia.Cep
                              ,Assistencia.Telefone
                              ,Assistencia.Telefone_2
                              ,Assistencia.Telefone_3
                              ,Assistencia.Telefone_4
                              ,Assistencia.Email
                              ,Assistencia.IdCidade
                              ,Assistencia.IdEstado
                              ,Assistencia.Lat
                              ,Assistencia.Long
                              ,Assistencia.Ativo
                              ,Assistencia.Idlinha
                              ,Estado.Sigla as NomeEstado
                           ,Cidade.Descricao as NomeCidade
                        FROM Assistencia 
                        JOIN Cidade ON Cidade.Id = Assistencia.IdCidade
                           JOIN Estado ON Estado.Id = Assistencia.IdEstado
    
                        WHERE Assistencia.Id = @id
                ", new { id }).FirstOrDefault();
            }
        }

        public bool Salvar(Assistencia model)
        {
            try
            {
                if (model.Id != Guid.Empty)
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                            UPDATE [dbo].[Assistencia]
                               SET [Id] = @Id
                                  ,[Nome] = @Nome
                                  ,[Rua] = @Rua
                                  ,[Numero] = @Numero
                                  ,[Bairro] = @Bairro
                                  ,[Complemento] = @Complemento
                                  ,[Cep] = @Cep
                                  ,[Telefone] = @Telefone
                                  ,[Telefone_2] = @Telefone_2
                                  ,[Telefone_3] = @Telefone_3
                                  ,[Telefone_4] = @Telefone_4
                                  ,[Email] = @Email
                                  ,[IdCidade] = @IdCidade
                                  ,[IdEstado] = @IdEstado
                                  ,[Lat] = @Lat
                                  ,[Long] = @Long
                                  ,[IdLinha] = @IdLinha
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
                            INSERT INTO [dbo].[Assistencia]
                                       ([Id]
                                       ,[Nome]
                                       ,[Rua]
                                       ,[Numero]
                                       ,[Bairro]
                                       ,[Complemento]
                                       ,[Cep]
                                       ,[Telefone]
                                       ,[Telefone_2]
                                       ,[Telefone_3]
                                       ,[Telefone_4]
                                       ,[Email]
                                       ,[IdCidade]
                                       ,[IdEstado]
                                       ,[Lat]
                                       ,[Long]
                                       ,[IdLinha]
                                       ,[Ativo])
                                 VALUES
                                       (@Id
                                       ,@Nome
                                       ,@Rua
                                       ,@Numero
                                       ,@Bairro
                                       ,@Complemento
                                       ,@Cep
                                       ,@Telefone
                                       ,@Telefone_2
                                       ,@Telefone_3
                                       ,@Telefone_4
                                       ,@Email
                                       ,@IdCidade
                                       ,@IdEstado
                                       ,@Lat
                                       ,@Long
                                       ,@IdLinha
                                       ,@Ativo)
                        ", model);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        public List<Assistencia> ListarBuscarCidade(Guid IdLinha, string uf, int idCidade)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Assistencia>(
                @"
                        SELECT Assistencia.Id
                              ,Assistencia.Nome
                              ,Assistencia.Rua
                              ,Assistencia.Numero
                              ,Assistencia.Bairro
                              ,Assistencia.Complemento
                              ,Assistencia.Cep
                              ,Assistencia.Telefone
                              ,Assistencia.Telefone_2
                              ,Assistencia.Telefone_3
                              ,Assistencia.Telefone_4
                              ,Assistencia.Email
                              ,Assistencia.IdCidade
                              ,Assistencia.IdEstado
                              ,Assistencia.Lat
                              ,Assistencia.Long
                              ,Assistencia.IdLinha
                              ,Assistencia.Ativo
                            
                        FROM 
                            Assistencia 
                  
                        WHERE
                            
                            Assistencia.IdLinha = @idLinha AND
                            Assistencia.IdEstado = @uf AND
                            Assistencia.IdCidade = @idCidade
    
                ", new { IdLinha, uf, idCidade }).ToList();
            }

        }

        public List<Assistencia> ListarBuscar(Guid IdLinha)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Assistencia>(
                @"
                        SELECT Assistencia.Id
                              ,Assistencia.Nome
                              ,Assistencia.Rua
                              ,Assistencia.Numero
                              ,Assistencia.Bairro
                              ,Assistencia.Complemento
                              ,Assistencia.Cep
                              ,Assistencia.Telefone
                              ,Assistencia.Telefone_2
                              ,Assistencia.Telefone_3
                              ,Assistencia.Telefone_4
                              ,Assistencia.Email
                              ,Assistencia.IdCidade
                              ,Assistencia.IdEstado
                              ,Assistencia.Lat
                              ,Assistencia.Long
                              ,Assistencia.IdLinha
                               ,Assistencia.Ativo
                            
                        FROM 
                            Assistencia 
                  
                        WHERE
                            
                            Assistencia.IdLinha = @idLinha
    
                ", new { IdLinha }).ToList();
            }

        }

        public List<Assistencia> Listar()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Assistencia>(
                @"
                        SELECT Assistencia.Id
                              ,Assistencia.Nome
                              ,Assistencia.Rua
                              ,Assistencia.Numero
                              ,Assistencia.Bairro
                              ,Assistencia.Complemento
                              ,Assistencia.Cep
                              ,Assistencia.Telefone
                              ,Assistencia.Telefone_2
                              ,Assistencia.Telefone_3
                              ,Assistencia.Telefone_4
                              ,Assistencia.Email
                              ,Assistencia.IdCidade
                              ,Assistencia.IdEstado
                              ,Assistencia.Lat
                              ,Assistencia.Long
                              ,Assistencia.Ativo
                               ,Estado.Sigla as NomeEstado
                           ,Cidade.Descricao as NomeCidade
                        FROM Assistencia 
                        JOIN Cidade ON Cidade.Id = Assistencia.IdCidade
                           JOIN Estado ON Estado.Id = Assistencia.IdEstado
    
                ").ToList();
            }
        }
    }
}
