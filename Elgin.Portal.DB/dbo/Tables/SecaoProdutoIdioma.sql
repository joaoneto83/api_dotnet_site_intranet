CREATE TABLE [dbo].[SecaoProdutoIdioma] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_SecaoProdutoIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdSecaoProduto] UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma]   INT              NOT NULL,
    [Texto1]         VARCHAR (200)    NULL,
    [Texto2]         VARCHAR (500)    NULL,
    [Texto3]         VARCHAR (4000)   NULL,
    [CodigoVideo]    VARCHAR (50)     NULL,
    [CodigoVideo2]   VARCHAR (50)     NULL,
    [CodigoVideo3]   VARCHAR (50)     NULL,
    [CodigoVideo4]   VARCHAR (50)     NULL,
    CONSTRAINT [PK_SecaoProdutoIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

