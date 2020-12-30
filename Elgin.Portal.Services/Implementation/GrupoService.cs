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
    public class GrupoService : AbstractService<GrupoService>
    {
        private UsuarioService usuarioService;

        public GrupoService(string connectionString, FileSettings fileSettings) : base(connectionString)
        {
            usuarioService = new UsuarioService(connectionString, fileSettings);
        }

        public List<Grupo> ListarGrupos(string termoPesquisado)
        {
            if (string.IsNullOrEmpty(termoPesquisado) == false)
                termoPesquisado = "%" + termoPesquisado + "%";

            var retorno = new List<Grupo>();

            var sql = @"
                    SELECT Grupo.Id
                          ,NomeGrupo
                          ,(SELECT CONVERT(varchar, Grupo.DataCriacao, 103)) AS DataCriacao
	                      ,Usuario.Nome
                          ,Usuario.Id
	                      ,Arquivo.Caminho AS CaminhoFoto
                    FROM Grupo
                    LEFT JOIN GrupoUsuario 
                    ON Grupo.Id = GrupoUsuario.IdGrupo
                    LEFT JOIN Usuario 
                    ON GrupoUsuario.IdUsuario = Usuario.Id
                    LEFT JOIN Arquivo
                    ON Arquivo.Id = Usuario.IdArquivo
                    WHERE Grupo.Ativo = 1 AND
	                    (Grupo.NomeGrupo like @termoPesquisado 
                         OR @termoPesquisado is null)
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, Grupo>();

                retorno = conexao.Query<Grupo, Usuario, Grupo>(sql,
                (grupo, usuario) =>
                {
                    Grupo grupoEntry;

                    if (!dictionary.TryGetValue(grupo.Id, out grupoEntry))
                    {
                        grupoEntry = grupo;
                        grupoEntry.Pessoas = new List<Usuario>();
                        dictionary.Add(grupoEntry.Id, grupoEntry);
                    }

                    if (usuario != null && !grupoEntry.Pessoas.Any(x => x.Id == usuario.Id))
                        grupoEntry.Pessoas.Add(usuario);

                    return grupoEntry;

                }, new { termoPesquisado },
                splitOn: "Nome").Distinct().ToList();
            }

            return retorno;
        }

        public Grupo ProcuraGrupoPorId(Guid idGrupo)
        {
            var objGrupo = RetornaGrupo(idGrupo);

            objGrupo.Pessoas = usuarioService.ListarUsuarioPorGrupo(idGrupo);

            return objGrupo;
        }

        private Grupo RetornaGrupo(Guid idGrupo)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Grupo>(
                @"SELECT Id
                        ,NomeGrupo
                        ,Ativo
                    FROM Grupo
                  WHERE Id = @idGrupo 
                ", new { idGrupo }).FirstOrDefault();
            }
        }

        public List<Grupo> RetornaGrupoPorIdUsuario(Guid idUsuario)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Grupo>(
                @"SELECT Grupo.Id
                        ,Grupo.NomeGrupo
                        ,Grupo.Ativo
                    FROM Grupo
                    LEFT JOIN GrupoUsuario 
                    ON GrupoUsuario.IdGrupo = Grupo.Id
                  WHERE GrupoUsuario.IdUsuario = @idUsuario AND Grupo.Ativo = 1
                ", new { idUsuario }).ToList();
            }
        }

        public bool Salvar(Grupo model)
        {
            try
            {
                if (model.Id != Guid.Empty)
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             UPDATE [Grupo]
                               SET [NomeGrupo] = @NomeGrupo
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
                             INSERT INTO [Grupo]
                                        ([Id]
                                        ,[NomeGrupo]
                                        ,[Ativo]
                                        ,[DataCriacao])
                                  VALUES
                                        (@Id
                                        ,@NomeGrupo
                                        ,@Ativo
                                        ,getDate())
                        ", model);
                    }
                }

                RemoverUsuarios(model);
                SalvarUsuarios(model);

            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private void SalvarUsuarios(Grupo model)
        {
            foreach (var pessoa in model.Pessoas)
            {
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Execute(
                    @"IF (NOT EXISTS (SELECT 1 FROM [GrupoUsuario] WHERE IdUsuario = @idUsuario AND IdGrupo = @idGrupo))
                        BEGIN
                            INSERT INTO [GrupoUsuario]
                                ([IdGrupo]
                                ,[IdUsuario])
                            VALUES
                                (@idGrupo
                                ,@idUsuario)
                        END

                ", new { idUsuario = pessoa.Id, idGrupo = model.Id });
                }
            }
        }

        private void RemoverUsuarios(Grupo model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                       DELETE [GrupoUsuario]
                         WHERE IdUsuario NOT IN @Ids AND IdGrupo = @IdGrupo

                ", new
                {
                    Ids = model.Pessoas.Select(x => x.Id).ToList(),
                    IdGrupo = model.Id
                });
            }
        }

        public bool InativarGrupo(Guid idGrupo)
        {
            try
            {
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Query(
                    @"UPDATE Grupo SET Ativo = 0 WHERE Id = @idGrupo 
                    ", new { idGrupo });
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
