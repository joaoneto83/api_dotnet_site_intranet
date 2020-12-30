CREATE TABLE [dbo].[Pedido] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [NumeroPedido]      INT              IDENTITY (1, 1) NOT NULL,
    [ValorTotal]        DECIMAL (10, 2)  NULL,
    [NomeCompleto]      VARCHAR (250)    NULL,
    [Endereco]          VARCHAR (250)    NULL,
    [Numero]            VARCHAR (10)     NULL,
    [Cep]               VARCHAR (8)      NULL,
    [Cidade]            VARCHAR (100)    NULL,
    [Estado]            VARCHAR (100)    NULL,
    [Bairro]            VARCHAR (100)    NULL,
    [Telefone]          VARCHAR (20)     NULL,
    [Email]             VARCHAR (250)    NULL,
    [Cpf]               VARCHAR (50)     NULL,
    [Rg]                VARCHAR (15)     NULL,
    [IsPessoaFisica]    BIT              NULL,
    [RazaoSocial]       VARCHAR (250)    NULL,
    [Cnpj]              VARCHAR (20)     NULL,
    [InscricaoEstadual] VARCHAR (10)     NULL,
    [IdTipoPagamento]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Pedido] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Pedido_TipoPagamento] FOREIGN KEY ([IdTipoPagamento]) REFERENCES [dbo].[TipoPagamento] ([Id])
);

