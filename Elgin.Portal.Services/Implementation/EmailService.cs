using Elgin.Portal.Services.Implementation.Shared;
using System;

using System.Net.Mail;
using System.Net;
using Elgin.Portal.Services.Model;
using System.IO;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Transactions;
using System.Threading.Tasks;

namespace Elgin.Portal.Services.Implementation
{
    public class EmailService : AbstractService<AssistenciaService>
    {
        private DominioService dominioService;
        private ArquivoService arquivoService;
        private UsuarioService usuarioService;
        private LinhaService linhaService;
        private SetorService setorService;

        public EmailService(string connectionString, EmailSettings emailSettings, FileSettings fileSettings) : base(connectionString, emailSettings)
        {
            dominioService = new DominioService(connectionString);
            arquivoService = new ArquivoService(connectionString, fileSettings);
            usuarioService = new UsuarioService(connectionString, fileSettings);
            linhaService = new LinhaService(connectionString);
            setorService = new SetorService(connectionString);
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

        public void EnviarEmail(Email model)
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

            msg.Subject = model.Assunto;
            msg.Body = model.Corpo;
            msg.IsBodyHtml = true;

            smtpClient.Send(msg);
            SalvarLogEmail(msg.Subject, msg.To.ToString());
        }



        public void NovoUsuario(Usuario model)
        {
            var email = new Email();

            email.Assunto = "Elgin - novo usuário";

            if (model.IdPerfil == Guid.Parse("d19ec9f1-db69-409c-88f7-c2fceb31a2ab"))
            {

                email.Para.Add("kaio.cesar@elgin.com.br");

            }
            else if (model.IdPerfil == Guid.Parse("1ab65ffe-40b3-42ed-a6ef-b512579724c6"))
            {

                email.Para.Add("carlos.souza@elgin.com.br");

            }
            else if (model.IdPerfil == Guid.Parse("1ab65ffe-40b3-42ed-a6ef-b512579724c8"))
            {

                email.Para.Add("carlos.souza@elgin.com.br");

            }
            else if (model.IdPerfil == Guid.Parse("2ab65ffe-40b3-42ed-a6ef-b512579724c9"))
            {

                email.Para.Add("carlos.souza@elgin.com.br");

            }

            else if (model.IdPerfil == Guid.Parse("5c38ffdd-4c4f-4efb-8ae5-cbd2bd0f6bbe"))
            {

                email.Para.Add("jefferson.leite@elgin.com.br");

            }

            else if ( model.IdPerfil == Guid.Parse("b6115d80-2cc8-484a-b300-0be482fe5f3e") ) {

                email.Para.Add("andrea.onishi@elgin.com.br");

            }
            else {
                email.Para.Add("simone.santos@elgin.com.br");
            }
            

            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = RetornaModeloEmail("NovoUsuario");

            modeloEmail = string.Format(modeloEmail, model.Nome, model.Login);

            email.Corpo = modeloHeader + modeloEmail + "</html>";

            EnviarEmailAsync(email);
        }




        public void EnviarSejaAssistencia(FormularioAssistenciaTecnica formulario)
        {
            var email = new Email();

       
            email.Assunto = "Elgin - SOLICITAÇÃO DE CREDENCIAMENTO DE AUTORIZADA";
            //foreach (var para in EmailSettings.CredenciamentoTo)
            //{
            //    email.Para.Add(para);
            //}
            if (formulario.produtos == "Climatizacao")
            {

                email.Para.Add("rita.oliveira@elgin.com.br");

            }
            else if (formulario.produtos == "Costura")
            {

                email.Para.Add("rita.oliveira@elgin.com.br");

            }

            else if (formulario.produtos == "EnergiaSolar")
            {

                email.Para.Add("rita.oliveira@elgin.com.br");

            }

            else if (formulario.produtos == "Automacao")
            {

                email.Para.Add("nelson.romero@elgin.com.br");

            }
            else if (formulario.produtos == "Refrigeracao")
            {

                email.Para.Add("refrigeracao@elgin.com.br");

            }
      
            else if (formulario.produtos == "Gelatta")
            {

                email.Para.Add("refrigeracao@elgin.com.br");

            }
            else
            {
                email.Para.Add("refrigeracao@elgin.com.br");
            }

            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = RetornaModeloEmail("CredenciamentoAutorizada");

            modeloEmail = string.Format(modeloEmail, formulario.razaoSocial, formulario.email,  formulario.telefone, formulario.cidade, formulario.bairro, formulario.estado, formulario.produtos, formulario.comentarios);

            email.Corpo = modeloHeader + modeloEmail + "</html>";

            EnviarEmailAsync(email);
        }


        public void EnviarPedidoGelatta(Pedido pedido)
        {
            var email = new Email();

            email.Assunto = "Elgin - Seu pedido foi concluído com sucesso!";

            email.Para.Add(pedido.Email);

            foreach (var para in EmailSettings.ContatoGelattaTo)
            {
                email.Para.Add(para);
            }

            var nomePedido = pedido.IsPessoaFisica ? pedido.NomeCompleto : pedido.RazaoSocial;
            string strProdutos = "";

            foreach (var produto in pedido.Produtos.Where(x => x.QtdProduto > 0))
            {
                strProdutos +=
                            $@"<tr style='font-family: ""Montserrat"",sans-serif; color:#222222;font-size:10px;font-weight:200;'>
                                    <td style=' padding: 0 2px;'>{produto.NomeProduto}</td>
                                    <td style=' padding: 0 2px; text-align: center;'><b style='font-weight:bold'>Valor unitário</b><br>{produto.Preco.ToString("C2")}</td>
                                    <td style=' padding: 0 2px; text-align: center;'><b style='font-weight:bold'>Quantidade</b><br>{produto.QtdProduto}</td>
                                    <td style=' padding: 0 2px;  text-align: center;'><b style='font-weight:bold'>Subtotal</b><br>{(produto.Preco * produto.QtdProduto).ToString("C2")}</td>
                                </tr>
                                <tr style='height: 10px;'></tr>";
            }

            email.Corpo = $@"
                       <!DOCTYPE html PUBLIC '-//W3C//DTD HTML 4.0 Transitional//EN' 'http://www.w3.org/TR/REC-html40/loose.dtd'>
<html style='background-color: #FAFAFA'>

<head>
    <link href='https://fonts.googleapis.com/css?family=Montserrat&display=swap' rel='stylesheet'>
</head>

<body>
    <table width='100%' border='0' cellspacing='0' cellpadding='0' style='background-color:#FFFFFF'>
        <tr>
            <td align='center'>
                <table style='width:575px;'>
                    <tr style='height:75px;'>
                        <td> <img src='http:/172.19.65.8:2008//assets/img/Email/Logo.jpg' alt='ELGIN'> </td>
                        <td align='right'> <a style='text-decoration:none;'
                                href='https://pt-br.facebook.com/GrupoElgin/'> <img class='icon' alt='Facebook'
                                    src='http:/172.19.65.8:2008//assets/img/Email/facebook_email.jpg'> </a>
                            <a style='text-decoration:none;' href='https://www.instagram.com/grupo_elgin/'> <img
                                    class='icon' alt='Instagram'
                                    src='http:/172.19.65.8:2008//assets/img/Email/instagram_email.jpg'> </a>
                            <a style='text-decoration:none;'
                                href='https://www.youtube.com/channel/UC4EEMFjqb3zQcn0vpE0ewAg'> <img class='icon'
                                    alt='Youtube'
                                    src='http:/172.19.65.8:2008//assets/img/Email/youtube_email.jpg'> </a>
                            <a style='text-decoration:none;' href='https://br.linkedin.com/company/elgin-s.a.-brazil-'>
                                <img class='icon' alt='LinkedIn'
                                    src='http:/172.19.65.8:2008//assets/img/Email/linkedin_email.jpg'> </a>
                            <a style='text-decoration:none;' href='http://www.elgin.com.br/blog/'> <img class='icon'
                                    alt='Blog'
                                    src='http:/172.19.65.8:2008//assets/img/Email/blog_email.jpg'> </a>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table style='background-color: #006198' width='100%'>
        <tr style='height:50px;'>
            <td>&nbsp;</td>
        </tr>
    </table>
    <table width='100%' cellspacing='0' cellpadding='0' style='background-color: #006198' ;border-top: solid 40px
        #006198'>
        <tr>
            <td align='center'>
                <table style='background-color:#FFFFFF;width:575px;'>
                    <tr>
                        <td colspan='2' align='center'>
                            <table width='61%' border='0' cellspacing='0' cellpadding='0'>
                                <tr>
                                    <td align='center'>
                                <tr style=''>
                                    <td style='padding-top:55px;padding-bottom: 30px'> <img
                                            src='http:/172.19.65.8:2008//assets/img/Email/Credenciamento/elgin_mail.jpg'
                                            alt='Icone'> </td>
                                    <td
                                        style='font-family: ""Montserrat"",sans-serif; color:#000000;font-size:21px;font-weight:200;text-align:left;padding-top:55px; padding-left:40px;'>
                                         Seu pedido foi recebido com sucesso!  <span
                                            style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;'>Elgin
                                            Website</span> </td>
                                </tr>
                                <tr>
                                    <td colspan='2' style='border-top:1px solid #B9B9B9;'> &nbsp; </td>
                                </tr>
                                <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                        Nome Completo </td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                        {pedido.NomeCompleto} </td>
                                </tr>
                                <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                        CPF </td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                         {pedido.Cpf} </td>
                                </tr>
                                <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                        E-mail </td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                         {pedido.Email} </td>
                                </tr>
                                <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                        Telefone </td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                        {pedido.Telefone} </td>
                                </tr>
                           
                                <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                        Cidade </td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                        {pedido.Cidade} </td>
                                </tr>
                                <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                        Estado </td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                        {pedido.Estado} </td>
                                </tr>
                                <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                        Endereço </td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                        {pedido.Endereco} </td>
                                </tr>
                                <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                        CEP </td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                        {pedido.Cep} </td>
                                </tr>
                                <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                        CNPJ </td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                        {pedido.Cnpj} </td>
                                </tr>
                                <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                       Pedido número: {pedido.NumeroPedido.ToString().PadLeft(5, '0')}</td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                        {strProdutos}</td>
                                </tr>
                                <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                        Total do pedido </td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                        {pedido.ValorTotal.ToString("C2")} </td>
                                </tr>
                               <tr style='padding-top: 35px'>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#A5A5A5;font-size:18px;font-weight:400;text-align:center;padding-top:12px'>
                                        forma de pagamento </td>
                                </tr>
                                <tr>
                                    <td colspan='2'
                                        style='font-family: ""Montserrat"",sans-serif; color:#000;font-size:14px;font-weight:400;text-align:center;'>
                                        {pedido.CodigoTipoPagamento} </td>
                                </tr>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table style='background-color:#FFFFFF;width:575px;    margin: 0 auto;'>
        <tr>
            <td
                style='font-family: ""Montserrat"",sans-serif; color:#9C9C9C;font-size:10px;font-weight:200;text-align:center;height:auto; padding: 30px;'>
                Por favor não responda este e-mail. Para entrar em contato com nosso time de
                                                                atendimento ao cliente envie um e-mail para gelatta@elgin.com.br informando o
                                                                número do pedido. </td>
        </tr>
    </table>
    </td>
    </tr>
    </table>
    <table style='background-color: #006198' width='100%'>
        <tr style='height:50px;'>
            <td>&nbsp;</td>
        </tr>
    </table>
</body>
</ html>";

            EnviarEmailAsync(email);
        }





        public void EnviarResetarSenha(string para, string codigo)
        {
            var email = new Email();

            email.Assunto = "Portal de Apoio - Alteração de Senha";
            email.Para.Add(para);

            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = RetornaModeloEmail("ResetarSenha");

            modeloEmail = string.Format(modeloEmail, codigo);

            email.Corpo = modeloHeader + modeloEmail + "</html>";

            EnviarEmailAsync(email);
        }


        public void EnviarContato(Contato model, int? idioma)
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

            if (model.Tipo == "SuporteTecnico")
            {
                msg.Subject = "Elgin - Suporte Técnico - Contato";

                foreach (var email in EmailSettings.ContatoTo)
                {
                    msg.To.Add(new MailAddress(email));
                }
            }
            else if (model.Tipo == "Climatizacao")
            {
                msg.Subject = "Elgin - Climatização - Contato";

                foreach (var email in EmailSettings.ContatoTo)
                {
                    msg.To.Add(new MailAddress(email));
                }
            }
            else if (model.Tipo == "Costura")
            {
                msg.Subject = "Elgin - Costura - Contato";

                foreach (var email in EmailSettings.ContatoTo)
                {
                    msg.To.Add(new MailAddress(email));
                }
            }
            else if (model.Tipo == "MaquinaSorvete")
            {
                msg.Subject = "Elgin - Máquina de Sorvete - Contato";

                foreach (var email in EmailSettings.ContatoRefrigeracaoTo)
                {
                    msg.To.Add(new MailAddress(email));
                }
            }
            else if (model.Tipo == "Automacao")
            {
                msg.Subject = "Elgin - Automação - Contato";

                foreach (var email in EmailSettings.ContatoAutomacaoTo)
                {
                    msg.To.Add(new MailAddress(email));
                }
            }

            else if (model.Tipo == "Informatica")
            {
                msg.Subject = "Elgin - Informática - Contato";

                foreach (var email in EmailSettings.ContatoInformaticaTo)
                {
                    msg.To.Add(new MailAddress(email));
                }
            }
            else if (model.Tipo == "Telefonia")
            {
                msg.Subject = "Elgin - Telefonia - Contato";

                foreach (var email in EmailSettings.ContatoTelefonia)
                {
                    msg.To.Add(new MailAddress(email));
                }
            }
            else if (model.Tipo == "ElginPay")
            {
                msg.Subject = "Elgin - Elgin Pay - Contato";

                foreach (var email in EmailSettings.ContatoAutomacaoTo)
                {
                    msg.To.Add(new MailAddress(email));
                }
            }
            else if (model.Tipo == "EnergiaSolar")
            {
                msg.Subject = "Elgin - Energia Solar - Contato";

                foreach (var email in EmailSettings.ContatoInformaticaTo)
                {
                    msg.To.Add(new MailAddress(email));
                }
            }
            else if (model.Tipo == "Iluminacao")
            {
                msg.Subject = "Elgin - Iluminação - Contato";

                foreach (var email in EmailSettings.ContatoInformaticaTo)
                {
                    msg.To.Add(new MailAddress(email));
                }
            }
            else if (model.Tipo == "Refrigeracao")
            {
                if (idioma == 2)
                    msg.Subject = "Elgin - Refrigeration - Contact";

                else if (idioma == 3)
                    msg.Subject = "Elgin - Refrigeración - Contacto";

                else
                    msg.Subject = "Elgin - Refrigeração - Contato";

                foreach (var email in EmailSettings.ContatoRefrigeracaoTo)
                {
                    msg.To.Add(new MailAddress(email));
                }
            }

            Guid idLinha;
            string corpoLinha = "";
            if (Guid.TryParse(model.IdLinha, out idLinha))
            {
                var linha = linhaService.ProcurarPorId(idLinha);
                corpoLinha = $"{linha.NomeLinha}";
            }

            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = string.Empty;

            if(model.Tipo == "Automacao")
            {
                modeloEmail = RetornaModeloEmail("ContatoSuporteTecnico");
                modeloEmail = string.Format(modeloEmail, model.Nome, model.EmpresaAreaAtuacao, model.Email, model.Telefone, model.Assunto, model.Mensagem);
            }
   
            else if(model.Tipo == "Refrigeracao")
            {
                if (idioma == 2)
                    modeloEmail = RetornaModeloEmail("ContatoSuporteTecnicoEN");

                else if (idioma == 3)
                    modeloEmail = RetornaModeloEmail("ContatoSuporteTecnicoES");

                else
                    modeloEmail = RetornaModeloEmail("ContatoSuporteTecnico");

                modeloEmail = string.Format(modeloEmail, model.Nome, model.EmpresaAreaAtuacao, model.Email, model.Telefone, model.Assunto, model.Mensagem);
            }
            else
            {
                modeloEmail = RetornaModeloEmail("Contato");
                modeloEmail = string.Format(modeloEmail, model.Nome, model.Cpf, model.Email, model.Telefone, corpoLinha, model.Mensagem);
            }

            msg.Body = modeloHeader + modeloEmail + "</html>";

            msg.IsBodyHtml = true;

            smtpClient.Send(msg);
            SalvarLogEmail(msg.Subject, msg.To.ToString());
        }

        public void SalvarContato(Contato model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    INSERT INTO [dbo].[Contato]
                           ([Nome]
                           ,[CpfCnpj]
                           ,[Email]
                           ,[Telefone]
                           ,[Tipo]
                           ,[IdLinha]
                           ,[Mensagem]
                           ,[Assunto]
                           ,[EmpresaAreaAtuacao])
                     VALUES
                           (@Nome
                           ,@Cpf
                           ,@Email
                           ,@Telefone
                           ,@Tipo
                           ,@IdLinha
                           ,@Mensagem
                           ,@Assunto
                           ,@EmpresaAreaAtuacao)
                ", model);
            }
        }

        public void EnviarDocumento(Guid idArquivo, Guid idUsuario, string emailPara)
        {
            var email = new Email();

            email.Assunto = "Elgin - Arquivo para Donwload";
            email.Para.Add(emailPara);

            Arquivo arquivo = arquivoService.GetArquivo(idArquivo);
            Usuario usuario = usuarioService.ProcurarPorId(idUsuario);

            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = RetornaModeloEmail("EnviarArquivo");

            modeloEmail = string.Format(modeloEmail, arquivo.NomeArquivo, Path.GetExtension(arquivo.Caminho).Replace(".", "").ToUpper(), arquivo.Caminho, usuario.Nome, usuario.Email);

            email.Corpo = modeloHeader + modeloEmail + "</html>";

            EnviarEmailAsync(email);
        }

        public void EnviarResultadoSolar(CalculoSolarResultado model)
        {
            var estado = dominioService.EstadoPorUF(model.calculo.estado);
            var cidade = dominioService.CidadePorId(model.calculo.idCidade);
            var listaIntegradores = dominioService.getSolarIntegradores(model.calculo.estado, model.calculo.idCidade);
            var onde = model.calculo.onde == "C" ? "Casa" : "Empresa";

            var email = new Email();

            email.Assunto = "Elgin - Simulação Kit Solar";
            email.Para.Add(model.email);
            if (EmailSettings.SolarCco != null)
                foreach (var item in EmailSettings.SolarCco)
                {
                    email.CCo.Add(item);
                }


            var kits = $@"Kit {model.kit1.ToString("N")} kw";
            var temTotal = "none";

            if (model.kit2 > 0)
            {
                temTotal = "block";
                kits += $@" + {model.kit2.ToString("N")} kw";
            }

            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = RetornaModeloEmail("CalculoSolar");
            var modeloEmailBlocoEconomia = RetornaModeloEmail("SolarBlocoEconomia");
            var modeloEmailBlocoIntegradores = RetornaModeloEmail("SolarBlocoIntegradores");

            var blocoEconomiaEnergia = string.Format(modeloEmailBlocoEconomia, model.economiaEnergia.ToString("N"));

            var blocoIntegradores = "";
            foreach (var integrador in listaIntegradores)
            {
                blocoIntegradores += string.Format(modeloEmailBlocoIntegradores, integrador.Nome, integrador.Endereco, integrador.Telefone);
            }

            email.Corpo = modeloHeader + string.Format(modeloEmail, model.condicaoSolar, model.estimativaArea.ToString("N"), blocoEconomiaEnergia, kits, temTotal, model.kitTotal.ToString("N"), cidade.Descricao, model.calculo.estado, model.economiaFinanceira.ToString("N"), blocoIntegradores) + "</html>";

            EnviarEmailAsync(email);
        }

        public void EnviarSejaIntegrador(FormularioIntegrador formulario)
        {
            var email = new Email();

            email.Assunto = "Elgin - SEJA INTEGRADOR";
            email.Para.Add("gabriela.santana@elgin.com.br");
            //foreach (var para in EmailSettings.CredenciamentoIntegradorTo)
            //{
            //    email.Para.Add(para);
            //}

            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = RetornaModeloEmail("CredenciamentoIntegrador");

            modeloEmail = string.Format(modeloEmail, formulario.razaoSocial, formulario.nomeFantasia, formulario.email, formulario.telefone, formulario.celular, formulario.cidade, formulario.estado, formulario.endereco, formulario.cep, formulario.inscricaoEstadual, formulario.cnpj, formulario.descricao);

            email.Corpo = modeloHeader + modeloEmail + "</html>";

            EnviarEmailAsync(email);
        }

        public void EnviarSolicitacaoRH(FormularioSolicitacao formulario)
        {
            var email = new Email();

            email.Assunto = "Elgin - SOLICITAÇÃO RH";
            foreach (var para in EmailSettings.SolicitacaoRhTo)
            {
                email.Para.Add(para);
            }

            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = RetornaModeloEmail("RhSolicitacao");

            modeloEmail = string.Format(modeloEmail, formulario.nomeCompleto, formulario.registro, formulario.setor, formulario.email, formulario.telefone, formulario.solicitacao);

            email.Corpo = modeloHeader + modeloEmail + "</html>";

            EnviarEmailAsync(email);
        }

        public void EnviarSugestaoTI(FormularioSugestao formulario)
        {
            var email = new Email();

            email.Assunto = "Elgin - SUGESTÃO TI";
            foreach (var para in EmailSettings.SugestaoTiTo)
            {
                email.Para.Add(para);
            }

            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = RetornaModeloEmail("TiSugestao");

            modeloEmail = string.Format(modeloEmail, formulario.nomeCompleto, formulario.registro, formulario.setor, formulario.email, formulario.telefone, formulario.sugestao);

            email.Corpo = modeloHeader + modeloEmail + "</html>";

            EnviarEmailAsync(email);
        }

        public void EnviarLojaFuncionario(FormularioLojaFuncionario formulario)
        {
            var email = new Email();

            email.Assunto = "Elgin - LOJA FUNCIONÁRIO";
            foreach (var para in EmailSettings.LojaFuncionarioTo)
            {
                email.Para.Add(para);
            }

            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = RetornaModeloEmail("LojaFuncionario");

            modeloEmail = string.Format(modeloEmail, formulario.nomeCompleto, formulario.email, formulario.telefone, formulario.setor, formulario.produto, formulario.modeloProduto, formulario.codigoCatalogo, formulario.quantidade, formulario.observacoes);

            email.Corpo = modeloHeader + modeloEmail + "</html>";

            EnviarEmailAsync(email);

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

        public void EnviarQueroComprar(FormularioQueroComprar formulario)
        {
            var email = new Email();

            email.Assunto = "Elgin - Automação - Quero Comprar";
            foreach (var para in EmailSettings.QueroComprarTo)
            {
                email.Para.Add(para);
            }

            var modeloHeader = RetornaModeloEmail("HeaderMontSerrat");
            var modeloEmail = RetornaModeloEmail("QueroComprar");

            modeloEmail = string.Format(modeloEmail, formulario.razaoSocial, formulario.nomeFantasia, formulario.email, formulario.telefone, formulario.celular, formulario.cidade, formulario.estado, formulario.endereco, formulario.cep, formulario.cnpj, formulario.descricao);

            email.Corpo = modeloHeader + modeloEmail + "</html>";

            EnviarEmailAsync(email);
        }
    }
}
