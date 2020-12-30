CREATE TABLE [dbo].[Classificacao] (
    [Id]                      UNIQUEIDENTIFIER CONSTRAINT [DF_Classificacao_Id] DEFAULT (newid()) NOT NULL,
    [NomeClassificacao]       VARCHAR (50)     NULL,
    [IdSubLinha]              UNIQUEIDENTIFIER NULL,
    [Ativo]                   BIT              NULL,
    [IdClassificacaoSuperior] UNIQUEIDENTIFIER NULL,
    [CaminhoImagem]           VARCHAR (80)     NULL,
    [OrdemImagem]             INT              NULL,
    [Comparativo]             BIT              NULL,
    CONSTRAINT [PK_Classificacao] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Classificacao_Categoria] FOREIGN KEY ([IdSubLinha]) REFERENCES [dbo].[SubLinha] ([Id]),
    CONSTRAINT [FK_Classificacao_Classificacao] FOREIGN KEY ([IdClassificacaoSuperior]) REFERENCES [dbo].[Classificacao] ([Id])
);







