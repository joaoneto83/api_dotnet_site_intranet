CREATE TABLE [dbo].[Produto] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [CodigoLegado]  VARCHAR (50)     NULL,
    [NomeProduto]   VARCHAR (100)    NULL,
    [IdSubLinha]    UNIQUEIDENTIFIER NULL,
    [Ativo]         BIT              NULL,
    [CodigoProduto] VARCHAR (100)    NULL,
    [Preco]         DECIMAL (10, 2)  NULL,
    CONSTRAINT [PK_Produto] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Produto_Categoria] FOREIGN KEY ([IdSubLinha]) REFERENCES [dbo].[SubLinha] ([Id])
);






GO

GO
