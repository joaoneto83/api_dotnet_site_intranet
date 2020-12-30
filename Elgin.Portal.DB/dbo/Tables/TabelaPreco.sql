CREATE TABLE [dbo].[TabelaPreco] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [NomeTabelaPreco] VARCHAR (150)    NULL,
    [Ativo]           BIT              NULL,
    CONSTRAINT [PK_TabelaPreco] PRIMARY KEY CLUSTERED ([Id] ASC)
);

