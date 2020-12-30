CREATE TABLE [dbo].[EspecificacaoTecnicaIdioma] (
    [Id]                     UNIQUEIDENTIFIER CONSTRAINT [DF_EspecificacaoTecnicaIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdEspecificacaoTecnica] UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma]           INT              NOT NULL,
    [NomeEspecificacao]      VARCHAR (100)    NULL,
    CONSTRAINT [PK_EspecificacaoTecnicaIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

