CREATE TABLE [dbo].[ProdutoEspecificacaoTecnica] (
    [IdProduto]              UNIQUEIDENTIFIER NOT NULL,
    [IdEspecificacaoTecnica] UNIQUEIDENTIFIER NOT NULL,
    [Valor]                  VARCHAR (100)    NULL,
    CONSTRAINT [PK_ModeloEspecificacaoTecnica_1] PRIMARY KEY CLUSTERED ([IdProduto] ASC, [IdEspecificacaoTecnica] ASC),
    CONSTRAINT [FK_ModeloEspecificacaoTecnica_EspecificacaoTecnica] FOREIGN KEY ([IdEspecificacaoTecnica]) REFERENCES [dbo].[EspecificacaoTecnica] ([Id]),
    CONSTRAINT [FK_ModeloEspecificacaoTecnica_Produto] FOREIGN KEY ([IdProduto]) REFERENCES [dbo].[Produto] ([Id])
);

