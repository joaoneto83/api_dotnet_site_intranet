CREATE TABLE [dbo].[TipoPagamento] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [CodigoTipoPagamento] VARCHAR (50)     NULL,
    [NomeTipoPagamento]   VARCHAR (100)    NULL,
    CONSTRAINT [PK_TipoPagamento] PRIMARY KEY CLUSTERED ([Id] ASC)
);

