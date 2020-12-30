CREATE TABLE [dbo].[ArquivoIdioma] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_ArquivoIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdArquivo]    UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma] INT              NOT NULL,
    [NomeArquivo]  VARCHAR (150)    NULL,
    CONSTRAINT [PK_ArquivoIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

