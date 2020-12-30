using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.Transactions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Elgin.Portal.Services.Implementation
{
    public class UsuarioService : AbstractService<UsuarioService>
    {
        private ArquivoService arquivoService;
        private EmailService emailService;
        private DependenteService dependenteService;

        public UsuarioService(string connectionString, FileSettings fileSettings) : base(connectionString)
        {
            arquivoService = new ArquivoService(connectionString, fileSettings);
            dependenteService = new DependenteService(connectionString);
        }

        public UsuarioService(string connectionString, EmailSettings emailSettings, FileSettings fileSettings) : base(connectionString)
        {
            arquivoService = new ArquivoService(connectionString, fileSettings);
            emailService = new EmailService(connectionString, emailSettings, fileSettings);
            dependenteService = new DependenteService(connectionString);
        }



        public List<Usuario> Listar()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Usuario>(
                @"
                    SELECT [Id]
                          ,[Nome]
                          ,[Login]
                          ,[Email]
                          ,[IdPerfil]
                          ,[UltimoAcesso]
                          ,[Ativo]
                    FROM [Usuario]
                ", new { }).ToList();
            };
        }

        public List<Usuario> ListarUsuarios(string termoPesquisado)
        {
            if (string.IsNullOrEmpty(termoPesquisado) == false)
                termoPesquisado = "%" + termoPesquisado + "%";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Usuario>(
                @"
                    SELECT DISTINCT Usuario.[Id]
                          ,Usuario.[Nome]
                          ,Usuario.[Login]
                          ,Usuario.[Email]
                          ,Usuario.[IdPerfil]
                          ,Usuario.[IdArquivo]
                          ,Usuario.[UltimoAcesso]
                          ,Usuario.[Ativo]
                          ,Arquivo.Caminho AS CaminhoFoto
                          ,(SELECT Nome FROM Perfil WHERE Id = Usuario.IdPerfil) AS NomePerfil
                          ,(STUFF((SELECT DISTINCT ', ' + Grupo.NomeGrupo AS [text()]
					               FROM Grupo
					               JOIN GrupoUsuario
					               ON Grupo.Id =  GrupoUsuario.IdGrupo
					               WHERE GrupoUsuario.IdUsuario = Usuario.Id AND Grupo.Ativo = 1
					               For XML PATH ('')), 1, 1, '')) as NomeGrupos
                    FROM [Usuario]
                    LEFT JOIN Arquivo
                    ON Arquivo.Id = Usuario.IdArquivo
                    LEFT JOIN GrupoUsuario 
                    ON GrupoUsuario.IdUsuario = Usuario.Id
                    LEFT JOIN Grupo
                    ON Grupo.Id = GrupoUsuario.IdGrupo
                    WHERE Usuario.Ativo = 1 AND
	                    (Usuario.Nome like @termoPesquisado 
                    OR Usuario.Login like @termoPesquisado
                    OR Usuario.Email like @termoPesquisado
                    OR Grupo.NomeGrupo like @termoPesquisado
                    OR @termoPesquisado is null)

                ", new { termoPesquisado }).ToList();
            };
        }

        public List<Usuario> ListarUsuarioPorGrupo(Guid idGrupo)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Usuario>(
                @"
                    SELECT Usuario.[Id]
                          ,Usuario.[Nome]
                          ,Usuario.[Login]
                          ,Usuario.[Email]
                          ,Usuario.[IdPerfil]
                          ,Usuario.[IdArquivo]
                          ,Usuario.[UltimoAcesso]
                          ,Usuario.[Ativo]
                          ,Arquivo.Caminho AS CaminhoFoto
                    FROM [Usuario]
                    LEFT JOIN Arquivo
                    ON Arquivo.Id = Usuario.IdArquivo
                    LEFT JOIN GrupoUsuario 
                    ON GrupoUsuario.IdUsuario = Usuario.Id
                    LEFT JOIN Grupo
                    ON Grupo.Id = GrupoUsuario.IdGrupo
                    WHERE Grupo.Id = @idGrupo

                ", new { idGrupo }).ToList();
            };
        }

        public Usuario ProcurarPorId(Guid id)
        {
            var retorno = new Usuario();
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Usuario>(
                @"
                    SELECT Usuario.[Id]
                          ,Usuario.[Nome]
                          ,Usuario.[Login]
                          ,Usuario.[Email]
                          ,Usuario.[IdPerfil]
                          ,Usuario.[IdArquivo]
                          ,Usuario.[UltimoAcesso]
                          ,Usuario.[Ativo]
                          ,Usuario.[IdSetor]
                          ,Usuario.[Telefone]
                          ,Usuario.[Registro]
                          ,Usuario.[Endereco]
                          ,Usuario.[Bairro]
                          ,Usuario.[Cidade]
                          ,Usuario.[Escolaridade]
                          ,Usuario.[EstadoCivil]
                          ,Usuario.[IdTipoUsuario]
                          ,Usuario.[ComoSerChamado]
                          ,Usuario.[EmailAniversariante]
                          ,Usuario.[Celular]
                          ,Usuario.[ContatoEmergencia]
                          ,Usuario.[NomeContatoEmergencia]
                          ,(SELECT CONVERT(varchar, Usuario.[DataNascimento], 23)) AS DataNascimento
                          ,Arquivo.Caminho AS CaminhoFoto
                          ,Perfil.Nome as NomePerfil
                          ,Setor.NomeSetor as NomeSetor
                    FROM [Usuario]
                    LEFT JOIN Arquivo
                    ON Arquivo.Id = Usuario.IdArquivo
                    LEFT JOIN PERFIL
                    ON Perfil.Id = Usuario.IdPerfil
                    LEFT JOIN Setor
                    ON Setor.Id = Usuario.IdSetor
                   WHERE Usuario.id = @id
                ", new { id }).FirstOrDefault();
            };

            retorno.Dependentes = dependenteService.ListarPorIdUsuario(retorno.Id);

            retorno.Arquivo = arquivoService.GetArquivoPorIdPaiTipo(retorno.Id, "curriculo");

            return retorno;
        }

        public List<Usuario> ListarAniversariantesMes()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Usuario>(
                    @"SELECT Usuario.[Id]
                          ,Usuario.[Nome]
                          ,Usuario.[Login]
                          ,Usuario.[Email]
                          ,Usuario.[IdPerfil]
                          ,Usuario.[IdArquivo]
                          ,Usuario.[UltimoAcesso]
                          ,Usuario.[Ativo]
                          ,(SELECT CONVERT(varchar, Usuario.[DataNascimento], 23)) AS DataNascimento
                          ,Arquivo.Caminho AS CaminhoFoto
                          ,Perfil.Nome as NomePerfil
                    FROM [Usuario]
                    LEFT JOIN Arquivo
                    ON Arquivo.Id = Usuario.IdArquivo
                    LEFT JOIN PERFIL
                    ON Perfil.Id = Usuario.IdPerfil
                    WHERE MONTH(Usuario.DataNascimento) = MONTH(getDate())
                    ORDER BY DAY(Usuario.DataNascimento)").Distinct().ToList();
            }
        }

        public List<Usuario> ListaTelefonica(string letra)
        {
            List<Usuario> retorno = new List<Usuario>();

            if (!string.IsNullOrEmpty(letra))
                letra = letra + "%";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Usuario>(
                    @"SELECT Usuario.[Id]
                          ,Usuario.[Nome]
                          ,Usuario.[Ativo]
                          ,Usuario.[Telefone]
                          ,(SELECT CONVERT(varchar, Usuario.[DataNascimento], 23)) AS DataNascimento
                          ,Arquivo.Caminho AS CaminhoFoto
                          ,Setor.NomeSetor as NomeSetor
                    FROM [Usuario]
                    LEFT JOIN Arquivo
                    ON Arquivo.Id = Usuario.IdArquivo
                    LEFT JOIN Setor
                    ON Setor.Id = Usuario.IdSetor
                    WHERE Usuario.Telefone IS NOT NULL AND Usuario.Telefone != ''
                      AND (Usuario.Nome like @letra OR @letra IS NULL)
                    ORDER BY Usuario.Nome", new { letra }).Distinct().ToList();
            }

            //foreach (var item in retorno)
            //{
            //    if (item.Telefone.Length <= 10)
            //        item.Telefone = string.Format("## ####-####", item.Telefone);
            //    else if (item.Telefone.Length == 11)
            //        item.Telefone = string.Format("## #####-####", item.Telefone);
            //}

            return retorno;
        }

        public bool ValidaLogin(Usuario model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return !conexao.Query<Guid>(
                @"
                   SELECT TOP 1 ID FROM Usuario WHERE Login = @Login AND Id <> @Id
                ", model).Any();
            }
        }

        public bool AlterarSenha(NovaSenha model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var retorno = conexao.Execute(
                @"
                    UPDATE [dbo].[Usuario]
                       SET [Senha] = @Senha, CodigoSenha = ''
                    WHERE Login = @Login AND [CodigoSenha] = @Codigo
                ", model);

                return retorno > 0;
            }
        }

        public Guid ValidarCodigo(string login, string codigo)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Guid>(
                @"
                    SELECT [Id] FROM Usuario WHERE Login = @login AND codigoSenha = @codigo
                ", new { login, codigo }).FirstOrDefault();
            }
        }

        public Usuario Login(string login, string senha)
        {
            var retorno = new List<Usuario>();

            var sql = @"
                      SELECT Usuario.[Id]
                          ,Usuario.[Nome]
                          ,Usuario.[Login]
                          ,Usuario.[Email]
                          ,Usuario.[IdPerfil]
                          ,Usuario.[Telefone]
                          ,Usuario.[Registro]
                          ,Setor.NomeSetor
                          ,Arquivo.Caminho AS CaminhoFoto
                          ,Funcionalidade.Codigo
                    FROM [Usuario]
                    LEFT JOIN Arquivo
                    ON Arquivo.Id = Usuario.IdArquivo
                    LEFT JOIN PerfilFuncionalidade PF
                    ON PF.IdPerfil = Usuario.IdPerfil
                    LEFT JOIN Funcionalidade
                    ON Funcionalidade.Id = PF.IdFuncionalidade
                    LEFT JOIN Setor
                    ON Setor.Id = Usuario.IdSetor
                    WHERE 
                        Usuario.Ativo = 1 AND
                        Usuario.Login = @login AND
                        Usuario.Senha = @senha
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, Usuario>();

                retorno = conexao.Query<Usuario, string, Usuario>(sql,
                (usuario, codigoAcesso) =>
                {
                    Usuario usuarioEntry;

                    if (!dictionary.TryGetValue(usuario.Id, out usuarioEntry))
                    {
                        usuarioEntry = usuario;
                        usuarioEntry.Acessos = new List<string>();
                        dictionary.Add(usuarioEntry.Id, usuarioEntry);
                    }

                    if (!string.IsNullOrEmpty(codigoAcesso))
                        usuarioEntry.Acessos.Add(codigoAcesso);

                    return usuarioEntry;
                },
                new { login, senha },
                splitOn: "Codigo").Distinct().ToList();
            }

            var retusuario = retorno.FirstOrDefault();

            if(retusuario != null)
                AtualizaUltimoAcesso(retusuario.Id);

            return retusuario;
        }

        private void AtualizaUltimoAcesso(Guid id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var retorno = conexao.Execute(
                @"
                    UPDATE [dbo].[Usuario]
                       SET UltimoAcesso = GETDATE()
                    WHERE id = @id
                ", new { id });
            }
        }

        public async Task<bool> SalvarAsync(Usuario model, IFormFile file, string caminho, string nomeArquivo, string idPai)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var novo = false;
                    if (model.Id == Guid.Empty)
                    {
                        novo = true;
                        model.Id = Guid.NewGuid();
                    }

                    if (string.IsNullOrEmpty(model.CaminhoFoto) || file != null)
                    {
                        //var usuarioOld = ProcurarPorId(model.Id);
                        //if (usuarioOld != null)
                        //    this.arquivoService.RemoverArquivo(usuarioOld.CaminhoFoto);

                        model.IdArquivo = null;
                    }

                    if (file != null)
                    {
                        var retornoFile = await arquivoService.salvarBlobAsync(file, caminho, nomeArquivo, model.Id.ToString());
                        if (retornoFile.sucesso)
                        {
                            var arquivo = arquivoService.AddArquivo(
                                new Arquivo
                                {
                                    Id = model.IdArquivo ?? Guid.Empty,
                                    Caminho = retornoFile.caminho,
                                    CodigoTipoArquivo = caminho,
                                    NomeArquivo = file.Name,
                                    Ativo = true,
                                    Ordem = 0
                                });

                            model.IdArquivo = arquivo.Id;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (!ValidaLogin(model))
                    {
                        return false;
                    }

                    if (novo)
                    {
                        model.CodigoSenha = RandomString(6);
                        IncluirUsuario(model);
                    }
                    else
                    {
                        EditarUsuario(model);
                    }

                    model.Arquivo.IdPai = model.Id;

                    SalvarCurriculo(model);

                    dependenteService.SalvarDependentes(model);

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }

            return true;
        }

        private void SalvarCurriculo(Usuario model)
        {
            if (model.Arquivo.Id == Guid.Empty)
            {
                var arquivoOld = arquivoService.GetArquivoPorIdPaiTipo(model.Id, "curriculo");

                if (arquivoOld != null)
                    arquivoService.DelArquivo(arquivoOld.Id);
            }

            arquivoService.AddArquivo(model.Arquivo);
        }

        public bool ResetarSenha(string login)
        {
            var CodigoSenha = RandomString(6);

            var retorno = 0;

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Execute(
                @"
                    UPDATE [dbo].[Usuario]
                       SET [CodigoSenha] = @CodigoSenha
                    WHERE Login = @login
                ", new { login, CodigoSenha });


            }

            if (retorno > 0)
            {
                var usuario = ProcuraPorLogin(login);

                emailService.EnviarResetarSenha(usuario.Email, usuario.CodigoSenha);

                return true;
            }

            return false;

        }

        private Usuario ProcuraPorLogin(string login)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Usuario>(
                @"
                    SELECT Usuario.[Id]
                          ,Usuario.[Nome]
                          ,Usuario.[Login]
                          ,Usuario.[Email]
                          ,Usuario.[CodigoSenha]
                          ,Usuario.[IdPerfil]
                          ,Usuario.[IdArquivo]
                          ,Usuario.[UltimoAcesso]
                          ,Usuario.[Ativo]
                    FROM [Usuario]
                   WHERE Usuario.Login = @login
                ", new { login }).FirstOrDefault();
            };
        }

        private void EditarUsuario(Usuario model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE [dbo].[Usuario]
                       SET [Id] = @Id
                          ,[Nome] = @Nome
                          ,[Login] = @Login
                          ,[Email] = @Email
                          ,[IdPerfil] = @IdPerfil
                          ,[IdArquivo] = @IdArquivo
                          ,[Ativo] = @Ativo
                          ,[DataNascimento] = @DataNascimento
                          ,[Telefone] = @Telefone
                          ,[IdSetor] = @IdSetor
                          ,[Registro] = @Registro
                          ,[Endereco] = @Endereco
                          ,[Bairro] = @Bairro
                          ,[Cidade] = @Cidade
                          ,[Escolaridade] = @Escolaridade
                          ,[EstadoCivil] = @EstadoCivil
                          ,[IdTipoUsuario] = @IdTipoUsuario
                          ,[ComoSerChamado] = @ComoSerChamado
                          ,[EmailAniversariante] = @EmailAniversariante
                          ,[Celular] = @Celular
                          ,[ContatoEmergencia] = @ContatoEmergencia
                          ,[NomeContatoEmergencia] = @NomeContatoEmergencia
                    WHERE Id = @Id
                ", model);
            }
        }

        private void IncluirUsuario(Usuario model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    INSERT INTO [dbo].[Usuario]
                           ([Id]
                           ,[Nome]
                           ,[Login]
                           ,[Email]
                           ,[IdPerfil]
                           ,[IdArquivo]
                           ,[CodigoSenha]
                           ,[Ativo]
                           ,[DataNascimento]
                           ,[Telefone]
                           ,[IdSetor]
                           ,[Registro]
                           ,[Endereco]
                           ,[Bairro]
                           ,[Cidade]
                           ,[Escolaridade]
                           ,[EstadoCivil]
                           ,[IdTipoUsuario]
                           ,[ComoSerChamado]
                           ,[EmailAniversariante]
                           ,[Celular]
                           ,[ContatoEmergencia]
                           ,[NomeContatoEmergencia])
                     VALUES
                           (@Id
                           ,@Nome
                           ,@Login
                           ,@Email
                           ,@IdPerfil
                           ,@IdArquivo
                           ,@CodigoSenha
                           ,@Ativo
                           ,@DataNascimento
                           ,@Telefone
                           ,@IdSetor
                           ,@Registro
                           ,@Endereco
                           ,@Bairro
                           ,@Cidade
                           ,@Escolaridade
                           ,@EstadoCivil
                           ,@IdTipoUsuario
                           ,@ComoSerChamado
                           ,@EmailAniversariante
                           ,@Celular
                           ,@ContatoEmergencia
                           ,@NomeContatoEmergencia
)
               ", model);
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
