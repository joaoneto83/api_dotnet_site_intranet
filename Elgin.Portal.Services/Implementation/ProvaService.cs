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
    public class ProvaService : AbstractService<ProvaService>
    {
        private AgendamentoService agendamentoService;
        private UsuarioService usuarioService;
        private GrupoService grupoService;

        public ProvaService(string connectionString, FileSettings fileSettings) : base(connectionString)
        {
            agendamentoService = new AgendamentoService(connectionString, fileSettings);
            usuarioService = new UsuarioService(connectionString, fileSettings);
            grupoService = new GrupoService(connectionString, fileSettings);
        }

        public List<Prova> ListarProvas()
        {
            var retorno = new List<Prova>();

            var sql = @"
                    SELECT 
                        Prova.Id
	                    ,Prova.Nome
                        ,Prova.Descricao
                        ,(SELECT CONVERT(varchar, DataProva, 103)) AS DataProva
                        ,Prova.IdUltimaAlteracao
                        ,Prova.DataUltimaAlteracao
                        ,Prova.Ativo
	                    ,(SELECT COUNT (Id) FROM Questao WHERE Questao.Ativo = 1 AND IdProva = Prova.Id) AS QtdQuestoes
                        ,CASE WHEN Prova.IdLinha IS NOT NULL
		                    THEN(SELECT NomeLinha FROM Linha WHERE Id = Prova.IdLinha)
		                    ELSE(SELECT NomeProduto FROM Produto WHERE id = Prova.IdProduto)
	                    END AS TipoProva
                    FROM Prova
                    WHERE Prova.Ativo = 1
                    ORDER BY TipoProva, Nome
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Prova>(sql).Distinct().ToList();
            }

            return retorno;
        }

        public TreinamentoUsuario RetornaTreinamentoUsuario(Guid idUsuario)
        {
            var model = new TreinamentoUsuario();

            model.Usuario = usuarioService.ProcurarPorId(idUsuario);
            model.GruposUsuario = grupoService.RetornaGrupoPorIdUsuario(idUsuario);
            model.ProvasUsuario = ListarProvasPorUsuario(idUsuario);
            model.ProvasGrafico = RetornaProvasGrafico(model);

            return model;
        }

        public ProvasGrafico RetornaProvasGrafico(TreinamentoUsuario model)
        {
            var retorno = new ProvasGrafico();
            retorno.ProvasTotal = model.ProvasUsuario.Count();
            retorno.ProvasRealizadas = model.ProvasUsuario.Where(x => x.TerminoProva.HasValue).Count();
            if (retorno.ProvasRealizadas > 0)
                retorno.Aproveitamento = (int)Math.Round(model.ProvasUsuario.Sum(x => x.Nota) / retorno.ProvasRealizadas, 0);

            return retorno;
        }

        public List<ProvaUsuario> ListarProvasPorUsuario(Guid idUsuario)
        {
            var retorno = new List<ProvaUsuario>();
            var sql = @"
                    SELECT 
	                   AgendamentoUsuario.[Id]
	                  ,AgendamentoUsuario.[IdAgendamento]
                      ,AgendamentoUsuario.[IdUsuario]
                      ,AgendamentoUsuario.[InicioProva]
                      ,AgendamentoUsuario.[TerminoProva]
                      ,AgendamentoUsuario.[Nota]
                      ,AgendamentoUsuario.[IdProva]
                      ,AgendamentoUsuario.[QtdAcertos]
                      ,Agendamento.[QtdQuestoes]
                      ,Agendamento.[DataDe]
                      ,Agendamento.[DataAte]
	                  ,Prova.[Nome]
                      ,Prova.[Descricao]
                      ,Prova.[DataProva]
                      ,Prova.[Ativo]
                      ,Prova.[IdLinha]
                      ,Prova.[IdProduto]
	                  ,Linha.NomeLinha
	                  ,Produto.NomeProduto
	                  ,Usuario.Nome as NomeUsuario
                  FROM [AgendamentoUsuario]
                  JOIN Agendamento
                  ON Agendamento.Id = AgendamentoUsuario.IdAgendamento
                  JOIN Prova
                  ON Prova.Id = AgendamentoUsuario.IdProva
                  JOIN Usuario
                  ON Usuario.Id = AgendamentoUsuario.IdUsuario
                  LEFT JOIN Linha
                  ON Linha.Id = IdLinha
                  LEFT JOIN Produto
                  ON Produto.Id = IdProduto
                  WHERE IdUsuario = @idUsuario
                  ORDER BY DataAte DESC, TerminoProva
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<ProvaUsuario>(sql, new { idUsuario }).Distinct().ToList();
            }

            return retorno;
        }

        public Resultado RetornaResultado(Resultado resultado)
        {
            if (resultado.DataAte == null && resultado.DataDe != null)
                resultado.DataAte = Convert.ToDateTime(resultado.DataDe).AddDays(1).ToString("yyyy-MM-dd");

            var provas = new List<ProvaUsuario>();

            var sql = @"
                    SELECT 
	                   AgendamentoUsuario.[Id]
	                  ,AgendamentoUsuario.[IdAgendamento]
                      ,AgendamentoUsuario.[IdUsuario]
                      ,AgendamentoUsuario.[TerminoProva]
                      ,AgendamentoUsuario.[Nota]
                      ,AgendamentoUsuario.[IdProva]
                  FROM [AgendamentoUsuario]
                  JOIN Agendamento
                  ON Agendamento.Id = AgendamentoUsuario.IdAgendamento
                  WHERE (Agendamento.IdProva = @IdProva OR @IdProva IS NULL)
                        AND (Agendamento.IdGrupo = @IdGrupo OR @IdGrupo IS NULL)
						AND (AgendamentoUsuario.TerminoProva BETWEEN @DataDe AND @DataAte 
                             OR @DataDe IS NULL OR @DataAte IS NULL)
                  ORDER BY TerminoProva DESC
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                provas = conexao.Query<ProvaUsuario>(sql, resultado).Distinct().ToList();
            }
            
            resultado.TotalProvas = provas.Count();
            resultado.PercentRealizados = provas.Where(x => x.TerminoProva.HasValue).Count();
            if (resultado.PercentRealizados > 0)
            {
                resultado.PercentAproveitamento = (int)Math.Round(provas.Sum(x => x.Nota) / resultado.PercentRealizados, 0);
                resultado.PercentRealizados = (int)Math.Round(resultado.PercentRealizados * 100M / resultado.TotalProvas);
            }
            else
                resultado.PercentAproveitamento = 0;

            return resultado;
        }

        public Prova ProcuraProvaPorId(Guid idProva)
        {
            var objProva = RetornaProva(idProva);

            objProva.Questoes = RetornaQuestoes(idProva);

            return objProva;
        }

        public bool InativarProva(Guid idProva)
        {
            try
            {
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Query(
                    @"UPDATE Prova SET Ativo = 0 WHERE Id = @idProva 
                    ", new { idProva });
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private List<Questao> RetornaQuestoes(Guid idProva)
        {
            var retorno = new List<Questao>();

            var sql = @"
                    SELECT 
                        Questao.Id
	                    ,Questao.Descricao
                        ,Questao.Ativo
                        ,Alternativa.Correta
                        ,Alternativa.Id
                        ,Alternativa.Ativo
                        ,Alternativa.Descricao
                    FROM Questao
                    LEFT JOIN Alternativa
                    ON Alternativa.IdQuestao = Questao.Id and Alternativa.Ativo = 1
                    WHERE Questao.Ativo = 1 AND	Questao.IdProva = @idProva
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, Questao>();

                retorno = conexao.Query<Questao, Alternativa, Questao>(sql,
                (questao, alternativa) =>
                {
                    Questao questaoEntry;

                    if (!dictionary.TryGetValue(questao.Id, out questaoEntry))
                    {
                        questaoEntry = questao;
                        questaoEntry.Alternativas = new List<Alternativa>();
                        dictionary.Add(questaoEntry.Id, questaoEntry);
                    }

                    if (alternativa != null)
                        questaoEntry.Alternativas.Add(alternativa);

                    return questaoEntry;

                }, new { idProva },
                splitOn: "Correta").Distinct().ToList();
            }

            return retorno;
        }

        private Prova RetornaProva(Guid idProva)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Prova>(
                @"SELECT Prova.[Id]
                      ,Prova.[Nome]
                      ,Prova.[Descricao]
                      ,(SELECT CONVERT(varchar, DataProva, 23)) AS DataProva
                      ,Prova.[IdUltimaAlteracao]
                      ,Prova.[DataUltimaAlteracao]
                      ,Prova.[Ativo]
                      ,Prova.[IdLinha]
                      ,Prova.[IdProduto]
                      ,Produto.NomeProduto
                      ,CASE WHEN Prova.[IdLinha] IS NOT NULL
		                    THEN(SELECT NomeLinha FROM Linha WHERE Id = Prova.IdLinha)
		                    ELSE(SELECT NomeProduto FROM Produto WHERE id = Prova.IdProduto)
	                    END AS TipoProva
                  FROM [Prova]
                  LEFT JOIN Produto
                  ON Produto.Id = Prova.IdProduto
                  WHERE Prova.Id = @idProva 
                ", new { idProva }).FirstOrDefault();
            }
        }

        public Prova AddProva(Guid idUsuario)
        {
            Prova model = new Prova();

            model.Id = Guid.NewGuid();
            model.Descricao = "";
            model.Nome = "";
            model.IdUltimaAlteracao = idUsuario;
            model.DataUltimaAlteracao = DateTime.Now;
            model.Ativo = true;

            return model;
        }

        public Questao AddQuestao(Questao questao)
        {
            questao.Id = Guid.NewGuid();
            questao.Descricao = "";
            questao.Alternativas = new List<Alternativa>();
            questao.Ativo = true;

            return questao;
        }

        public bool Salvar(Prova model)
        {
            try
            {
                AtualizarProva(model);

                AtualizarQuestoes(model);

                foreach (var questao in model.Questoes)
                {
                    AtualizarAlternativa(questao);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;

        }
        private void AtualizarProva(Prova model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    IF (NOT EXISTS (SELECT 1 FROM [Prova] WHERE Id = @Id))
                        BEGIN
                            INSERT INTO Prova
                                       (Id
                                       ,Nome
                                       ,Descricao
                                       ,DataProva
                                       ,IdUltimaAlteracao
                                       ,DataUltimaAlteracao
                                       ,Ativo
                                       ,IdLinha
                                       ,IdProduto)
                                 VALUES
                                       (@Id
                                       ,@Nome
                                       ,@Descricao
                                       ,@DataProva
                                       ,@IdUltimaAlteracao
                                       ,getDate()
                                       ,1
                                       ,@IdLinha
                                       ,@IdProduto)
                        END
                    ELSE
                        BEGIN
                            UPDATE [Prova]
                               SET [Nome] = @Nome
                                  ,[Descricao] = @Descricao
                                  ,[DataProva] = @DataProva
                                  ,[IdUltimaAlteracao] = @IdUltimaAlteracao
                                  ,[DataUltimaAlteracao] = getdate()
                                  ,[Ativo] = @Ativo
                                  ,[IdLinha] = @IdLinha
                                  ,[IdProduto] = @IdProduto
                             WHERE Id = @Id
                        END
                ", model);
            }
        }

        private void AtualizarQuestoes(Prova model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    IF (NOT EXISTS (SELECT 1 FROM [Questao] WHERE Id = @Id))
                        BEGIN
                            INSERT INTO Questao
                                       (Id
                                       ,Descricao
                                       ,IdProva
                                       ,Ativo)
                                 VALUES
                                       (@Id
                                       ,@Descricao
                                       ,@IdProva
                                       ,1)
                        END
                    ELSE
                        BEGIN

                            UPDATE [Questao]
                               SET [Descricao] = @Descricao
                                  ,[Ativo] = @Ativo
                             WHERE Id = @Id
                        END
                ", model.Questoes.Where(x => x.Id != Guid.Empty));
            }
        }

        public Alternativa AddAlternativa(Alternativa model)
        {
            model.Id = Guid.NewGuid();
            model.Descricao = "";
            model.Ativo = true;

            return model;
        }

        private void AtualizarAlternativa(Questao model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    IF (NOT EXISTS (SELECT 1 FROM [Alternativa] WHERE Id = @Id))
                        BEGIN
                        INSERT INTO [Alternativa]
                                   ([Id]
                                   ,[Descricao]
                                   ,[IdQuestao]
                                   ,[Correta]
                                   ,[Ativo])
                        VALUES
                                   (@Id
                                   ,@Descricao
                                   ,@IdQuestao
                                   ,@Correta
                                   ,@Ativo)
                        END
                    ELSE
                        BEGIN
                        UPDATE [Alternativa]
                           SET [Descricao] = @Descricao
                              ,[Correta] = @Correta
                              ,[Ativo] = @Ativo
                         WHERE Id = @Id
                        END
                ", model.Alternativas.Where(x => x.Id != Guid.Empty));
            }
        }

        public List<Questao> RetornaQuestoesAleatorias(Guid idProva, int qtdQuestoes, Guid idAgendamento, Guid idUsuario)
        {
            List<Questao> questoes;
            var ordem = 1;
            Guid idAgendamentoUsuario;

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                questoes = conexao.Query<Questao>(
                @"
                    SELECT TOP(@qtdQuestoes) 
                            Questao.Id
				            ,Questao.Descricao
				            ,Questao.Ativo
                            ,Questao.IdProva
                    FROM Questao
                    WHERE Questao.Ativo = 1 AND
	                      Questao.IdProva = @idProva
                    ORDER BY NEWID()
                ", new { idProva, qtdQuestoes, }).ToList();
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                idAgendamentoUsuario = conexao.Query<Guid>(
                @"
                    SELECT Id
                    FROM AgendamentoUsuario
                    WHERE IdAgendamento = @idAgendamento AND IdUsuario = @idUsuario
                ", new { idAgendamento, idUsuario }).FirstOrDefault();
            }

            foreach (var questao in questoes)
            {
                questao.Ordem = ordem;
                ordem++;

                questao.IdAgendamentoUsuario = idAgendamentoUsuario;

                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    questao.Alternativas = conexao.Query<Alternativa>(
                    @"Select Alternativa.Id
	                        ,Alternativa.Descricao
	                        ,Alternativa.Ativo
	                        ,Alternativa.IdQuestao
                    From Alternativa
                    Where Alternativa.IdQuestao = @Id
	                    AND Alternativa.Ativo = 1
                    ORDER BY NEWID()
                ", questao).ToList();
                }
            }

            return questoes;
        }

        public AgendamentoUsuario RetornaAgendamentoUsuarioPorId(Guid id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<AgendamentoUsuario>(
                @"
                    SELECT AgendamentoUsuario.[IdAgendamento]
                          ,AgendamentoUsuario.[IdUsuario]
                          ,AgendamentoUsuario.[InicioProva]
                          ,AgendamentoUsuario.[TerminoProva]
                          ,AgendamentoUsuario.[Nota]
                          ,AgendamentoUsuario.[IdProva]
                          ,AgendamentoUsuario.[Id]
                          ,AgendamentoUsuario.[TempoRestante]
                          ,AgendamentoUsuario.[QtdAcertos]
                      FROM [AgendamentoUsuario]
                    Where Id = @id
                ", new { id }).FirstOrDefault();
            }
        }

        public Prova AbrirProva(Guid idAgendamentoUsuario, Guid idUsuario)
        {
            var agendamentoUsuario = RetornaAgendamentoUsuarioPorId(idAgendamentoUsuario);

            var agendamento = agendamentoService.ProcuraAgendamentoPorId(agendamentoUsuario.IdAgendamento);

            var model = RetornaProva(agendamento.IdProva);

            if (string.IsNullOrEmpty(agendamentoUsuario.TerminoProva))
                model.ProvaAtiva = agendamento.ProvaAtiva;
            else
                model.ProvaAtiva = true;

            if (agendamentoUsuario.IdUsuario != idUsuario && string.IsNullOrEmpty(agendamentoUsuario.TerminoProva))
            {
                model.UsuarioCorreto = false;
                return model;
            }
            else
            {
                model.UsuarioCorreto = true;
            }

            if (model.ProvaAtiva == false) return model;

            model.ProvaRealizada = !string.IsNullOrEmpty(agendamentoUsuario.TerminoProva);

            if (VerificarProvaIniciada(agendamento.Id, agendamentoUsuario.IdUsuario) > 0)
            {
                model.Questoes = RetornaQuestoesProvaIniciada(agendamento.Id, agendamentoUsuario.IdUsuario);
            }
            else
            {
                var questoes = RetornaQuestoesAleatorias(agendamento.IdProva, agendamento.QtdQuestoes, agendamento.Id, agendamentoUsuario.IdUsuario);

                SalvarAgendamentoUsuario(questoes, agendamento.Id, agendamentoUsuario.IdUsuario);

                model.Questoes = RetornaQuestoesProvaIniciada(agendamento.Id, agendamentoUsuario.IdUsuario);
            }

            //model.TempoRestante = RetornaTempoRestante(agendamento.Id, idUsuario);

            return model;

        }

        private int RetornaTempoRestante(Guid idAgendamento, Guid idUsuario)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<int>(
                @"SELECT TempoRestante FROM AgendamentoUsuario
	                WHERE IdAgendamento = @idAgendamento
	                    AND IdUsuario = @idUsuario
                ", new { idAgendamento, idUsuario }).FirstOrDefault();
            }
        }

        private int VerificarProvaIniciada(Guid idAgendamento, Guid idUsuario)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<int>(
                @"SELECT COUNT(Id) FROM AgendamentoUsuario
	                WHERE IdAgendamento = @idAgendamento
	                    AND IdUsuario = @idUsuario
	                    AND InicioProva < getDate()
                ", new { idAgendamento, idUsuario }).FirstOrDefault();
            }
        }

        private void SalvarAgendamentoUsuario(List<Questao> model, Guid idAgendamento, Guid idUsuario)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"INSERT INTO [QuestaoAgendamentoUsuario]
                               ([IdQuestao]
                               ,[IdAgendamentoUsuario]
                               ,[Ordem])
                         VALUES
                               (@Id
                               ,@IdAgendamentoUsuario
                               ,@ordem)
                ", model);
            }



            foreach (var questao in model)
            {
                var ordem = 1;
                foreach (var alternativa in questao.Alternativas)
                {
                    alternativa.Ordem = ordem;
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"INSERT INTO [AlternativaQuestaoAgendamentoUsuario]
                                   ([IdAlternativa]
                                   ,[IdQuestaoAgendamentoUsuario]
                                   ,[Ordem])
                             VALUES
                                   (@idAlternativa
                                   ,(SELECT Id FROM QuestaoAgendamentoUsuario 
                                        WHERE IdQuestao = @idQuestao AND IdAgendamentoUsuario = @idAgendamentoUsuario)
                                   ,@ordem)
                ", new
                        {
                            idAlternativa = alternativa.Id,
                            idQuestao = questao.Id,
                            idAgendamentoUsuario = questao.IdAgendamentoUsuario,
                            ordem
                        });
                    }

                    ordem++;
                }
            }

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dataInicio = Util.PegaHoraBrasilia();

                conexao.Execute(
                @"UPDATE [AgendamentoUsuario]
                     SET [InicioProva] = @dataInicio
                  WHERE IdAgendamento = @idAgendamento AND IdUsuario = @idUsuario
                ", new { idAgendamento, idUsuario, dataInicio });
            }

        }

        public static DateTime PegaHoraBrasilia() => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        private List<Questao> RetornaQuestoesProvaIniciada(Guid idAgendamento, Guid idUsuario)
        {
            var retorno = new List<Questao>();

            var sql = @"SELECT QAU.Id AS IdQuestaoAgendamentoUsuario
	                          ,QAU.IdAgendamentoUsuario
	                          ,QAU.IdQuestao
                              ,Q.Id 
	                          ,Q.Descricao
	                          ,QAU.Ordem
                              ,AQAU.Id AS IdAlternativaQuestaoAgendamentoUsuario
	                          ,AQAU.Selecionada
                              ,A.Id
	                          ,A.Descricao
	                          ,A.IdQuestao
	                          ,AQAU.Ordem
                              ,CASE WHEN AU.TerminoProva IS NOT NULL
		                            THEN A.Correta
		                            ELSE null
	                            END AS Correta
                        FROM QuestaoAgendamentoUsuario as QAU
                        INNER JOIN Questao Q
                        ON Q.Id = QAU.IdQuestao
                        INNER JOIN AlternativaQuestaoAgendamentoUsuario AQAU
                        ON AQAU.IdQuestaoAgendamentoUsuario = QAU.Id
                        INNER JOIN Alternativa A
                        ON A.Id = AQAU.IdAlternativa
                        INNER JOIN AgendamentoUsuario AU
                        ON AU.Id = QAU.IdAgendamentoUsuario
                        WHERE AU.IdAgendamento = @idAgendamento AND
	                          AU.IdUsuario = @idUsuario
                        ORDER BY QAU.Ordem, AQAU.Ordem";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, Questao>();

                retorno = conexao.Query<Questao, Alternativa, Questao>(sql,
                (questao, alternativa) =>
                {
                    Questao questaoEntry;

                    if (!dictionary.TryGetValue(questao.Id, out questaoEntry))
                    {
                        questaoEntry = questao;
                        questaoEntry.Alternativas = new List<Alternativa>();
                        dictionary.Add(questaoEntry.Id, questaoEntry);
                    }

                    if (alternativa != null)
                        questaoEntry.Alternativas.Add(alternativa);

                    return questaoEntry;

                }, new { idAgendamento, idUsuario },
                splitOn: "IdAlternativaQuestaoAgendamentoUsuario").Distinct().ToList();
            }

            return retorno;
        }

        public void SalvarParcial(Questao model)
        {
            var prova = agendamentoService.ProcuraAgendamentoUsuarioPorId(model.IdAgendamentoUsuario.Value);

            if (!prova.ProvaFinalizada && !prova.ProvaRealizada)
            {
                AtualizarQuestoaUsuario(model);
            }
        }

        public Prova FinalizarProva(Prova model)
        {
            CalcularNota(model);
            AtualizarQuestoesUsuario(model.Questoes);
            AtualizarNotaUsuario(model);

            return model;
        }

        private void AtualizarQuestoaUsuario(Questao model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                    @"UPDATE [AlternativaQuestaoAgendamentoUsuario]
                             SET [Selecionada] = @Selecionada
                           WHERE Id = @IdAlternativaQuestaoAgendamentoUsuario"
                , model.Alternativas);
            }
        }

        private void AtualizarQuestoesUsuario(List<Questao> model)
        {
            foreach (var questao in model)
            {
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Execute(
                        @"UPDATE [AlternativaQuestaoAgendamentoUsuario]
                             SET [Selecionada] = @Selecionada
                           WHERE Id = @IdAlternativaQuestaoAgendamentoUsuario"
                    , questao.Alternativas);
                }
            }

        }

        private void AtualizarNotaUsuario(Prova model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dataAtual = Util.PegaHoraBrasilia();

                conexao.Execute(
                    @"UPDATE [dbo].[AgendamentoUsuario]
                         SET [TerminoProva] = @dataAtual
                            ,[Nota] = @Nota
                            ,[QtdAcertos] = @QtdAcertos
                      WHERE Id = @IdAgendamentoUsuario"
                , new { model.QtdAcertos, model.Questoes.First().IdAgendamentoUsuario, model.Nota, dataAtual });
            }
        }

        private void CalcularNota(Prova model)
        {
            foreach (var questao in model.Questoes)
            {
                foreach (var alternativa in questao.Alternativas)
                {
                    var idAlternativaCorreta = GetAlternativaCorreta(questao.Id);

                    if (alternativa.Selecionada && idAlternativaCorreta == alternativa.Id)
                    {
                        model.QtdAcertos++;
                        break;
                    }
                }
            }

            var nota = Convert.ToDecimal(model.QtdAcertos) * 100M / Convert.ToDecimal(model.Questoes.Count);

            model.Nota = Math.Round(nota, 2);
        }

        private Guid GetAlternativaCorreta(Guid idQuestao)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Guid>(
                    @"SELECT Id 
                        FROM Alternativa 
                       WHERE Correta = 1 AND IdQuestao = @idQuestao"
                , new { idQuestao }).FirstOrDefault();
            }
        }

        public ProvaUsuario RetornaAproveitamento(Guid idAgendamentoUsuario)
        {
            try
            {
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    var aproveitamento = conexao.Query<ProvaUsuario>(
                        @"SELECT 
                      AgendamentoUsuario.[Nota]
                      ,AgendamentoUsuario.[QtdAcertos]
                      ,Agendamento.[QtdQuestoes]
                  FROM [AgendamentoUsuario]
                  JOIN Agendamento
                  ON Agendamento.Id = AgendamentoUsuario.IdAgendamento
                  WHERE AgendamentoUsuario.Id = @idAgendamentoUsuario"
                       , new { idAgendamentoUsuario }).FirstOrDefault();

                    return aproveitamento;
                }
            }
            catch (Exception ex) { return null; }
        }

        public void FecharProvasFinalizadas()
        {
            var agendamentos = agendamentoService.ProcuraAgendamentoEmAberto();

            foreach (var agendamento in agendamentos)
            {
                var model = RetornaProva(agendamento.IdProva);
                model.Questoes = RetornaQuestoesProvaIniciada(agendamento.IdAgendamento, agendamento.IdUsuario);

                CalcularNota(model);
                AtualizarQuestoesUsuario(model.Questoes);
                AtualizarNotaUsuario(model);
            }
        }

    }
}
