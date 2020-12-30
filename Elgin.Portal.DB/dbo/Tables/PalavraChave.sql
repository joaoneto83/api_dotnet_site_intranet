CREATE TABLE [dbo].[PalavraChave] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_PalavraChave_Id] DEFAULT (newid()) NOT NULL,
    [Valor]     VARCHAR (150)    NULL,
    [IdProduto] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_PalavraChave] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PalavraChave_Produto] FOREIGN KEY ([IdProduto]) REFERENCES [dbo].[Produto] ([Id])
);




GO

GO

GO
