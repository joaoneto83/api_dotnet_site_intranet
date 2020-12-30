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
    public class PedidoService : AbstractService<PedidoService>
    {
        private EmailService emailService;

        public PedidoService(string connectionString, EmailSettings emailSettings,FileSettings fileSettings) : base(connectionString) {
            emailService = new EmailService(connectionString, emailSettings, fileSettings);
        }


        public int Salvar(Pedido pedido)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    pedido.Id = Guid.NewGuid();

                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        pedido.NumeroPedido = conexao.Query<int>(
                        @"INSERT INTO [dbo].[Pedido]
                                       ([Id]
                                       ,[ValorTotal]
                                       ,[NomeCompleto]
                                       ,[Endereco]
                                       ,[Numero]
                                       ,[Cep]
                                       ,[Cidade]
                                       ,[Estado]
                                       ,[Bairro]
                                       ,[Telefone]
                                       ,[Email]
                                       ,[Cpf]
                                       ,[Rg]
                                       ,[IsPessoaFisica]
                                       ,[RazaoSocial]
                                       ,[Cnpj]
                                       ,[InscricaoEstadual]
                                       ,[IdTipoPagamento])
                                 OUTPUT Inserted.NumeroPedido
                                 VALUES
                                       (@Id
                                       ,@ValorTotal
                                       ,@NomeCompleto
                                       ,@Endereco
                                       ,@Numero
                                       ,@Cep
                                       ,@Cidade
                                       ,@Estado
                                       ,@Bairro
                                       ,@Telefone
                                       ,@Email
                                       ,@Cpf
                                       ,@Rg
                                       ,@IsPessoaFisica
                                       ,@RazaoSocial
                                       ,@Cnpj
                                       ,@InscricaoEstadual
                                       ,(SELECT Id FROM TipoPagamento WHERE CodigoTipoPagamento = @CodigoTipoPagamento))", pedido).FirstOrDefault();
                    }

                    SalvarProdutos(pedido);

                    transaction.Complete();

                    emailService.EnviarPedidoGelatta(pedido);

                    return pedido.NumeroPedido;
                }
            }
            catch (Exception ex)
            {
            }

            return pedido.NumeroPedido;
        }

        private void SalvarProdutos(Pedido pedido)
        {
            foreach(var produto in pedido.Produtos)
            {
                using (SqlConnection conexao = new SqlConnection(ConnectionString))
                {
                    conexao.Execute(
                    @"INSERT INTO [PedidoProduto]
                                   ([IdPedido]
                                   ,[IdProduto]
                                   ,[QtdProduto]
                                   ,[ValorTotalProduto])
                             VALUES
                                   (@idPedido
                                   ,@idProduto
                                   ,@qtdProduto
                                   ,@valorTotalProduto)
                        ", new
                    {
                        idProduto = produto.Id,
                        idPedido = pedido.Id,
                        qtdProduto = produto.QtdProduto,
                        valorTotalProduto = produto.Preco * produto.QtdProduto
                    });
                }
            }
        }

        private void EnviarEmail(Pedido pedido)
        {

        }
    }
}
