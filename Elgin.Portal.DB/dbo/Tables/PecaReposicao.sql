CREATE TABLE [dbo].[PecaReposicao] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [Descricao]           VARCHAR (100)    NULL,
    [CodigoPecaReposicao] VARCHAR (50)     NULL,
    [Referencia]          VARCHAR (50)     NULL,
    [Preco]               DECIMAL (10, 2)  NULL,
    [IdProduto]           UNIQUEIDENTIFIER NOT NULL,
    [Ativo]               BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_PecaReposicao] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PecaReposicao_Produto] FOREIGN KEY ([IdProduto]) REFERENCES [dbo].[Produto] ([Id])
);

