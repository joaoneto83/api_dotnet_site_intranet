CREATE TABLE [dbo].[ProdutoClassificacao] (
    [IdProduto]       UNIQUEIDENTIFIER NOT NULL,
    [IdClassificacao] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ModeloClassificacao_1] PRIMARY KEY CLUSTERED ([IdProduto] ASC, [IdClassificacao] ASC),
    CONSTRAINT [FK_ModeloClassificacao_Classificacao] FOREIGN KEY ([IdClassificacao]) REFERENCES [dbo].[Classificacao] ([Id]),
    CONSTRAINT [FK_ModeloClassificacao_Produto] FOREIGN KEY ([IdProduto]) REFERENCES [dbo].[Produto] ([Id])
);

