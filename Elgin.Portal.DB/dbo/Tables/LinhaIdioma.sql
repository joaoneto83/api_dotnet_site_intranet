CREATE TABLE [dbo].[LinhaIdioma] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_LinhaIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdLinha]      UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma] INT              NOT NULL,
    [NomeLinha]    VARCHAR (50)     NULL,
    CONSTRAINT [PK_LinhaIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

