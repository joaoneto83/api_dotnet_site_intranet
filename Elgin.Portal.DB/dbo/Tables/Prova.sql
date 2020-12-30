CREATE TABLE [dbo].[Prova] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [Nome]                VARCHAR (150)    NULL,
    [Descricao]           VARCHAR (150)    NULL,
    [DataProva]           DATETIME         NULL,
    [IdUltimaAlteracao]   UNIQUEIDENTIFIER NOT NULL,
    [DataUltimaAlteracao] DATETIME         NULL,
    [Ativo]               BIT              NULL,
    [IdLinha]             UNIQUEIDENTIFIER NULL,
    [IdProduto]           UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Prova] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Prova_Linha] FOREIGN KEY ([IdLinha]) REFERENCES [dbo].[Linha] ([Id]),
    CONSTRAINT [FK_Prova_Produto] FOREIGN KEY ([IdProduto]) REFERENCES [dbo].[Produto] ([Id])
);



