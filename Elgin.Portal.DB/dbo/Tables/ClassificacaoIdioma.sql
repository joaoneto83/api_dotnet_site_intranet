CREATE TABLE [dbo].[ClassificacaoIdioma] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_ClassificacaoIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdClassificacao]   UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma]      INT              NOT NULL,
    [NomeClassificacao] VARCHAR (50)     NULL,
    CONSTRAINT [PK_ClassificacaoIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

