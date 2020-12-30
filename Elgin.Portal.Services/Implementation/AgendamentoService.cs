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
    public class AgendamentoService : AbstractService<AgendamentoService>
    {

        private UsuarioService usuarioService;

        public AgendamentoService(string connectionString, FileSettings fileSettings) : base(connectionString)
        {
            usuarioService = new UsuarioService(connectionString, fileSettings);
        }

        public List<Agendamento> ListarAgendamentos()
        {
            var retorno = new List<Agendamento>();

            var sql = @"
                SELECT Agendamento.Id
                        ,Agendamento.IdProva
                        ,(SELECT CONVERT(varchar, Agendamento.DataDe, 103)) AS DataDe
                        ,(SELECT CONVERT(varchar, Agendamento.DataAte, 103)) AS DataAte
                        ,Agendamento.QtdQuestoes
                        ,Agendamento.Duracao
                        ,Agendamento.Ativo
                        ,Agendamento.IdUsuarioAlteracao
                        ,Agendamento.IdGrupo
                        ,Agendamento.IdPessoa
                        ,Agendamento.Descricao
                        ,CASE WHEN Agendamento.IdGrupo IS NOT NULL
		                    THEN(SELECT Grupo.NomeGrupo FROM Grupo WHERE Id = Agendamento.IdGrupo)
		                    ELSE(SELECT Usuario.Nome FROM Usuario WHERE Id = Agendamento.IdPessoa)
	                    END AS NomeGrupoPessoa
                        ,Prova.Nome AS TituloProva
                    FROM Agendamento
                    left JOIN Prova ON
                    Prova.Id = Agendamento.IdProva
                    Where Agendamento.Ativo = 1
                    ORDER BY DataDe DESC

            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Agendamento>(sql).Distinct().ToList();
            }

            return retorno;
        }

        public AgendamentoUsuario ProcuraAgendamentoUsuarioPorId(Guid idAgendamentoUsuario)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<AgendamentoUsuario>(
                @"SELECT
		                AgendamentoUsuario.[Id]
		                ,AgendamentoUsuario.[IdAgendamento]
		                ,AgendamentoUsuario.[IdUsuario]
		                ,AgendamentoUsuario.[TempoRestante]
                        ,AgendamentoUsuario.InicioProva as dtInicioProva
		                ,AgendamentoUsuario.TerminoProva as dtTerminoProva
		                ,Agendamento.DataDe
		                ,Agendamento.DataAte
		                ,Agendamento.QtdQuestoes
		                ,Agendamento.Duracao
		                ,Agendamento.Descricao
		                ,Prova.Nome
                  FROM [DBELGIN_HMG].[dbo].[AgendamentoUsuario]
                  JOIN Agendamento
                  ON Agendamento.Id = AgendamentoUsuario.IdAgendamento
                  JOIN Prova
                  ON Prova.Id = Agendamento.IdProva
                  WHERE AgendamentoUsuario.Id = @idAgendamentoUsuario 
                ", new { idAgendamentoUsuario }).FirstOrDefault();
            }
        }

        public List<AgendamentoUsuario> ProcuraAgendamentoEmAberto()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dataAtual = Util.PegaHoraBrasilia();

                return conexao.Query<AgendamentoUsuario>(
                @"SELECT
	                    AgendamentoUsuario.[Id]
	                    ,AgendamentoUsuario.[IdAgendamento]
	                    ,AgendamentoUsuario.[IdUsuario]
	                    ,AgendamentoUsuario.[TempoRestante]
                        ,AgendamentoUsuario.InicioProva as dtInicioProva
	                    ,AgendamentoUsuario.TerminoProva as dtTerminoProva
	                    ,Agendamento.DataDe
	                    ,Agendamento.DataAte
	                    ,Agendamento.QtdQuestoes
	                    ,Agendamento.Duracao
	                    ,Agendamento.Descricao
	                    ,Prova.Nome
	                    ,Prova.ID as IdProva
                    FROM [dbo].[AgendamentoUsuario]
                    JOIN Agendamento
                    ON Agendamento.Id = AgendamentoUsuario.IdAgendamento
                    JOIN Prova
                    ON Prova.Id = Agendamento.IdProva
                    WHERE 
                    TerminoProva IS NULL AND 
                    InicioProva IS NOT NULL AND
                    @dataAtual > DATEADD(Minute, Duracao, InicioProva)
                ", new { dataAtual }).ToList();
            }
        }

        public Agendamento ProcuraAgendamentoPorId(Guid idAgendamento)
        {
            var objAgendamento = RetornaAgendamento(idAgendamento);

            return objAgendamento;
        }


        private Agendamento RetornaAgendamento(Guid idAgendamento)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Agendamento>(
                @"SELECT Agendamento.Id
                        ,Agendamento.IdProva
                        ,(SELECT CONVERT(varchar, Agendamento.DataDe, 23)) AS DataDe
                        ,(SELECT CONVERT(varchar, Agendamento.DataAte, 23)) AS DataAte
                        ,Agendamento.QtdQuestoes
                        ,Agendamento.Duracao
                        ,Agendamento.Ativo
                        ,Agendamento.IdUsuarioAlteracao
                        ,Agendamento.IdGrupo
                        ,Agendamento.IdPessoa
                        ,(CASE WHEN (GETDATE() >= Agendamento.DataDe AND GETDATE() < DATEADD(DAY,1,Agendamento.DataAte)) THEN 1 ELSE 0 END) AS ProvaAtiva
                        ,Usuario.Nome as NomeUsuario
                        ,Grupo.NomeGrupo
                        ,Agendamento.Descricao
                        ,CASE WHEN (SELECT COUNT(IdUsuario) FROM AgendamentoUsuario
				                      WHERE IdAgendamento = @idAgendamento AND InicioProva < getDate()) > 0
		                      THEN 1
		                      ELSE 0
	                      END AS ProvaStartada
                    FROM Agendamento
                    LEFT JOIN Usuario ON Usuario.Id = Agendamento.IdPessoa
                    LEFT JOIN Grupo ON Grupo.Id = Agendamento.IdGrupo
                  WHERE Agendamento.Id = @idAgendamento 
                ", new { idAgendamento }).FirstOrDefault();
            }
        }

        public bool Salvar(Agendamento model)
        {
            try
            {
                if (model.Id != Guid.Empty)
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             UPDATE [dbo].[Agendamento]
                               SET [IdProva] = @IdProva
                                  ,[DataDe] = @DataDe
                                  ,[DataAte] = @DataAte
                                  ,[QtdQuestoes] = @QtdQuestoes
                                  ,[Duracao] = @Duracao
                                  ,[Ativo] = @Ativo
                                  ,[IdUsuarioAlteracao] = @IdUsuarioAlteracao
                                  ,[IdGrupo] = @IdGrupo
                                  ,[IdPessoa] = @IdPessoa
                                  ,[Descricao] = @Descricao
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
                             INSERT INTO [Agendamento]
                                        ([Id]
                                        ,[IdProva]
                                        ,[DataDe]
                                        ,[DataAte]
                                        ,[QtdQuestoes]
                                        ,[Duracao]
                                        ,[Ativo]
                                        ,[IdUsuarioAlteracao]
                                        ,[IdGrupo]
                                        ,[IdPessoa]
                                        ,[Descricao])
                                  VALUES
                                        (@Id
                                        ,@IdProva
                                        ,@DataDe
                                        ,@DataAte
                                        ,@QtdQuestoes
                                        ,@Duracao
                                        ,@Ativo
                                        ,@IdUsuarioAlteracao
                                        ,@IdGrupo
                                        ,@IdPessoa
                                        ,@Descricao)
                        ", model);
                    }
                }

                if (model.IdPessoa != null && model.IdPessoa != Guid.Empty)
                {
                    RemoverAgendamentoUsuario(model.Id);
                    AddAgendamentoUsuario(model);
                }
                else if (model.IdGrupo != null && model.IdGrupo != Guid.Empty)
                {
                    var listaUsuarios = usuarioService.ListarUsuarioPorGrupo(model.IdGrupo.Value);

                    RemoverAgendamentoUsuario(model.Id);
                    AddAgendamentoUsuarioGrupo(listaUsuarios, model);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private void RemoverAgendamentoUsuario(Guid idAgendamento)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        DELETE [AgendamentoUsuario]
                         WHERE IdAgendamento = @IdAgendamento
                ", new
                {
                    IdAgendamento = idAgendamento
                });
            }
        }

        private void AtualizarAgendamentoUsuario(AgendamentoUsuario model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        UPDATE [AgendamentoUsuario]
                           SET [InicioProva] = @InicioProva
                              ,[TerminoProva] = @TerminoProva
                              ,[Nota] = @Nota
                         WHERE IdUsuario = @IdUsuario AND IdAgendamento = IdAgendamento
                ", model);
            }
        }

        private void AddAgendamentoUsuarioGrupo(List<Usuario> model, Agendamento agendamento)
        {
            foreach (var itens in model)
            {
                var newId = Guid.NewGuid();

                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Execute(
                    @"
                        INSERT INTO [AgendamentoUsuario]
                                   (
                                    [Id]
                                   ,[IdAgendamento]
                                   ,[IdUsuario]
                                   ,[IdProva]
                                   ,[TempoRestante])
                             VALUES
                                   (
                                    @id
                                   ,@idAgendamento
                                   ,@idUsuario
                                   ,@idProva
                                   ,@tempoRestante)
                ", new
                    {
                        id = newId,
                        idUsuario = itens.Id,
                        idProva = agendamento.IdProva,
                        idAgendamento = agendamento.Id,
                        tempoRestante = agendamento.Duracao
                    });
                }

                AdicionarNotificacao(newId, itens.Id, agendamento.DataDe, agendamento.DataAte);
            }
        }

        private void AddAgendamentoUsuario(Agendamento agendamento)
        {
            var newId = Guid.NewGuid();

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        INSERT INTO [AgendamentoUsuario]
                                   (
                                    [Id]
                                   ,[IdAgendamento]
                                   ,[IdUsuario]
                                   ,[IdProva]
                                   ,[TempoRestante])
                             VALUES
                                   (
                                    @id
                                   ,@idAgendamento
                                   ,@idUsuario
                                   ,@idProva
                                   ,@tempoRestante)
                ", new
                {
                    id = newId,
                    idUsuario = agendamento.IdPessoa.Value,
                    idProva = agendamento.IdProva,
                    idAgendamento = agendamento.Id,
                    tempoRestante = agendamento.Duracao
                });
            }

            AdicionarNotificacao(newId, agendamento.IdPessoa.Value, agendamento.DataDe, agendamento.DataAte);
        }

        private void AdicionarNotificacao(Guid idAgendamento, Guid idUsuario, string dataDe, string dataAte)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                        INSERT INTO [Notificacao]
                               ([IdUsuario]
                               ,[Tipo]
                               ,[Descricao]
                               ,[Link]
                               ,[DataInclusao])
                         VALUES
                               (@idUsuario
                               ,'Prova Agendada'
                               ,'Prova Agendada no período de ' + @dataDe + ' até ' + @dataAte
                               ,'/PortaldeApoio/Treinamentos/Prova/' + @idAgendamento
                               ,getDate())
                                    ", new
                {
                    idUsuario,
                    idAgendamento = idAgendamento.ToString(),
                    dataDe = Convert.ToDateTime(dataDe).ToString("dd/MM/yyyy"),
                    dataAte = Convert.ToDateTime(dataAte).ToString("dd/MM/yyyy")
                });
            }
        }

        public bool InativarAgendamento(Guid idAgendamento)
        {
            try
            {
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Query(
                    @"UPDATE Agendamento SET Ativo = 0 WHERE Id = @idAgendamento 
                    ", new { idAgendamento });
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
