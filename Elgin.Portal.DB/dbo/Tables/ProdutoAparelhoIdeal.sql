CREATE TABLE [dbo].[ProdutoAparelhoIdeal] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_RangeAparelhoIdeal_Id] DEFAULT (newid()) NOT NULL,
    [IdProduto] UNIQUEIDENTIFIER NOT NULL,
    [De]        DECIMAL (18, 2)  NOT NULL,
    [Ate]       DECIMAL (18, 2)  NOT NULL,
    CONSTRAINT [PK_RangeAparelhoIdeal] PRIMARY KEY CLUSTERED ([Id] ASC)
);

