CREATE TABLE [dbo].[ProdutoIdioma] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_ProdutoIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdProduto]    UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma] INT              NOT NULL,
    [NomeProduto]  VARCHAR (100)    NOT NULL,
    CONSTRAINT [PK_ProdutoIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);



