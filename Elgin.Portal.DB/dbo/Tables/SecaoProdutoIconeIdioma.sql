CREATE TABLE [dbo].[SecaoProdutoIconeIdioma] (
    [Id]                  UNIQUEIDENTIFIER CONSTRAINT [DF_SecaoProdutoIconeIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdSecaoProdutoIcone] UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma]        INT              NOT NULL,
    [DescricaoIcone]      VARCHAR (150)    NULL,
    [SubDescricaoIcone]   VARCHAR (150)    NULL,
    CONSTRAINT [PK_SecaoProdutoIconeIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

