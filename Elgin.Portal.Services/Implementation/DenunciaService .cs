using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;



using System.Net.Mail;
using System.Net;


using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.Transactions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Elgin.Portal.Services.Implementation
{
    public class DenunciaService : AbstractService<DenunciaService>
    {
        private ArquivoService arquivoService;
        private EmailService emailService;
        private DependenteService dependenteService;

        public DenunciaService(string connectionString, FileSettings fileSettings) : base(connectionString)
        {
            arquivoService = new ArquivoService(connectionString, fileSettings);
            dependenteService = new DependenteService(connectionString);
        }

        public DenunciaService(string connectionString, EmailSettings emailSettings, FileSettings fileSettings) : base(connectionString)
        {
            arquivoService = new ArquivoService(connectionString, fileSettings);
            emailService = new EmailService(connectionString, emailSettings, fileSettings);
            dependenteService = new DependenteService(connectionString);
        }



 
        public List<Denuncia> Listar()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Denuncia>(
                @"
                        SELECT Denuncia.Id
                              ,Denuncia.Nome
                              
                        FROM Denuncia 
                       
    
                ").ToList();
            }

        }


        public void enviarEmail(Denuncia model)
        {
            var email = new Email();

            email.Assunto = "Elgin - Denuncia";

            
            
            email.Para.Add("joaoneto83@gmail.com");
            


            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = RetornaModeloEmail("Denuncia");

            modeloEmail = string.Format(modeloEmail, model.protocolo);

            email.Corpo = modeloHeader + modeloEmail + "</html>";

        }

        public async void EnviarEmailAsync(Email model)
        {
            var smtpClient = new SmtpClient
            {
                Host = EmailSettings.PrimaryDomain,
                Port = EmailSettings.PrimaryPort,
                EnableSsl = true,
                Credentials = new NetworkCredential(EmailSettings.UsernameEmail, EmailSettings.UsernamePassword)
            };

            var msg = new MailMessage();
            msg.From = new MailAddress(EmailSettings.FromEmail);

            foreach (var para in model.Para)
            {
                msg.To.Add(new MailAddress(para));
            }

            if (model.CCo != null)
                foreach (var cco in model.CCo)
                {
                    msg.Bcc.Add(new MailAddress(cco));
                }

            msg.Subject = model.Assunto;
            msg.Body = model.Corpo;
            msg.IsBodyHtml = true;

            await smtpClient.SendMailAsync(msg);
            SalvarLogEmail(msg.Subject, msg.To.ToString());
        }



        private void SalvarLogEmail(string assunto, string destinatarios)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    var id = Guid.NewGuid();

                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                                INSERT INTO [dbo].[LogEmail]
                                    ([Id]
                                    ,[Destinatarios]
                                    ,[Assunto]
                                    ,[DataEnvio])
                                VALUES
                                    (@id
                                    ,@destinatarios
		                            ,@assunto
                                    ,@data)
                        ", new { id, destinatarios, assunto, data = DateTime.Now });
                    }
                    transaction.Complete();
                }
            }
            catch (Exception)
            {
            }
        }

        public string RetornaModeloEmail(string codigo)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<string>(
                @"
                    SELECT [Html]
                      FROM [dbo].[ModeloEmail]
                      WHERE Codigo = @codigo
                ", new { codigo }).FirstOrDefault();
            }
        }

        

        public Denuncia ProcurarPorProtocolo(string protocolo)
        {
            var retorno = new Denuncia();
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Denuncia>(
                @"
                    SELECT Denuncia.[Id]
                          ,Denuncia.[Nome]
                          ,Denuncia.[Empresa]
                          ,Denuncia.[Protocolo]
                          ,Denuncia.[permitir]
                          ,Denuncia.[ativo]
                          ,Denuncia.[telefone]
                          ,Denuncia.[celular]
                          ,Denuncia.[pergunta1]
                          ,Denuncia.[pergunta2]
                          ,Denuncia.[pergunta3]
                          ,Denuncia.[pergunta4]
                          ,Denuncia.[pergunta5]
                          ,Denuncia.[pergunta6]
                          ,Denuncia.[pergunta7]
                          ,Denuncia.[pergunta8]
                          ,Denuncia.[pergunta9]
                          ,Denuncia.[pergunta10]
                          ,Denuncia.[pergunta11]
                          ,Denuncia.[pergunta12]
                    FROM [Denuncia]

                   WHERE Denuncia.Protocolo = @Protocolo
                ", new { protocolo }).FirstOrDefault();
            };

            //LEFT JOIN Arquivo
            //ON Arquivo.Id = Denuncia.IdArquivo

            //retorno.Arquivo = arquivoService.GetArquivoPorIdPaiTipo(retorno.Id, "curriculo");

            return retorno;
        }






        //public async Task<bool> SalvarAsync(Denuncia model, IFormFile file, string caminho, string nomeArquivo, string idPai)
        //{
        //    try
        //    {
        //        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        //        {
        //            var novo = false;
        //            if (model.id == Guid.Empty)
        //            {
        //                novo = true;
        //                model.id = Guid.NewGuid();
        //            }


        //            if (file != null)
        //            {
        //                var retornoFile = await arquivoService.salvarBlobAsync(file, caminho, nomeArquivo, model.id.ToString());
        //                //if (retornoFile.sucesso)
        //                //{
        //                //    var arquivo = arquivoService.AddArquivo(
        //                //        new Arquivo
        //                //        {
        //                //            Id = model.IdArquivo ?? Guid.Empty,
        //                //            Caminho = retornoFile.caminho,
        //                //            CodigoTipoArquivo = caminho,
        //                //            NomeArquivo = file.Name,
        //                //            Ativo = true,
        //                //            Ordem = 0
        //                //        });

        //                //    model.IdArquivo = arquivo.Id;
        //                //}
        //                //else
        //                //{
        //                //    return false;
        //                //}
        //            }

        //            //if (novo)
        //            //{

        //            //    IncluirUsuario(model);
        //            //}


        //            //model.Arquivo.IdPai = model.id;

        //            //SalvarCurriculo(model);

        //            //dependenteService.SalvarDependentes(model);

        //            transaction.Complete();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return false;
        //    }

        //    return true;
        //}


        public bool Salvar(Denuncia model)
        {
            try
            {

                model.id = Guid.NewGuid();
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Execute(
                       @"
                            INSERT INTO [dbo].[Denuncia]
                           ([id]
                           ,[nome]
                           ,[empresa]
                           ,[email]
                           ,[celular]
                           ,[telefone]
                           ,[permitir]
                           ,[ativo]
                           ,[protocolo]
                           ,[pergunta1]
                           ,[pergunta2]
                           ,[pergunta3]
                           ,[pergunta4]
                           ,[pergunta5]
                           ,[pergunta6]
                           ,[pergunta7]
                           ,[pergunta8]
                           ,[pergunta9]
                           ,[pergunta10]
                           ,[pergunta11]
                           ,[pergunta12]
                          )
                     VALUES
                           (@id
                           ,@nome
                           ,@empresa
                           ,@email
                           ,@celular
                           ,@telefone
                           ,@permitir
                           ,@ativo
                           ,@protocolo
                           ,@pergunta1
                           ,@pergunta2
                           ,@pergunta3
                           ,@pergunta4
                           ,@pergunta5
                           ,@pergunta6
                           ,@pergunta7
                           ,@pergunta8
                           ,@pergunta9
                           ,@pergunta10
                           ,@pergunta11
                           ,@pergunta12
                           )
                        ", model);
                }
            
            }
            catch (Exception)
            {
                return true;
            }


            return true;
        }
           



        //private void IncluirUsuario(Denuncia model)
        //{
        //    using (SqlConnection conexao = new SqlConnection(ConnectionString))
        //    {
        //        conexao.Execute(
        //        @"
        //            INSERT INTO [dbo].[Denuncia]
        //                   ([Id]
        //                   ,[Nome]
        //                   ,[Empresa]
        //                   ,[Email]
        //                   ,[Celular]
        //                   ,[Telefone]
        //                   ,[Permitir]
        //                   ,[Ativo]
        //                   ,[Protocolo]
        //                   ,[Pergunta1]
        //                   ,[Pergunta2]
        //                   ,[Pergunta3]
        //                   ,[Pergunta4]
        //                   ,[Pergunta5]
        //                   ,[Pergunta6]
        //                   ,[Pergunta7]
        //                   ,[Pergunta8]
        //                   ,[Pergunta9]
        //                   ,[Pergunta10]
        //                   ,[Pergunta11]
        //                   ,[Pergunta12]
        //                 )
        //             VALUES
        //                   (@Id
        //                   ,@Nome
        //                   ,@Empresa
        //                   ,@Email
        //                   ,@Celular
        //                   ,@Telefone
        //                   ,@Permitir
        //                   ,@Ativo
        //                   ,@Pergunta1
        //                   ,@Pergunta2
        //                   ,@Pergunta3
        //                   ,@Pergunta4
        //                   ,@Pergunta5
        //                   ,@Pergunta6
        //                   ,@Pergunta7
        //                   ,@Pergunta8
        //                   ,@Pergunta9
        //                   ,@Pergunta10
        //                   ,@Pergunta11
        //                   ,@Pergunta12
        //                   )
        //       ", model);
        //    }
        //}

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
