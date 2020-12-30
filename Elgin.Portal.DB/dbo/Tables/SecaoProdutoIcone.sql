CREATE TABLE [dbo].[SecaoProdutoIcone] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_SecaoProdutoIcone_Id] DEFAULT (newid()) NOT NULL,
    [DescricaoIcone]    VARCHAR (150)    NULL,
    [IdSecaoProduto]    UNIQUEIDENTIFIER NULL,
    [IdIcone]           UNIQUEIDENTIFIER NULL,
    [Ordem]             INT              NULL,
    [SubDescricaoIcone] VARCHAR (150)    NULL,
    CONSTRAINT [PK_SecaoProdutoIcone] PRIMARY KEY CLUSTERED ([Id] ASC)
);



