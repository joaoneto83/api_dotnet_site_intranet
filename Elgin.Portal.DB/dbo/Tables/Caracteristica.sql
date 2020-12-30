CREATE TABLE [dbo].[Caracteristica] (
    [Id]                      UNIQUEIDENTIFIER CONSTRAINT [DF_Caracteristica_Id] DEFAULT (newid()) NOT NULL,
    [DescricaoCaracteristica] VARCHAR (150)    NULL,
    [IdProduto]               UNIQUEIDENTIFIER NULL,
    [CaracteristicaPrincipal] BIT              NULL,
    [IdIcone]                 UNIQUEIDENTIFIER NULL,
    [Ordem]                   INT              NULL,
    CONSTRAINT [PK_Caracteristica] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Caracteristica_Produto] FOREIGN KEY ([IdProduto]) REFERENCES [dbo].[Produto] ([Id])
);

