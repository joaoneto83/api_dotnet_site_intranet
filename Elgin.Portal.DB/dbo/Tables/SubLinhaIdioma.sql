CREATE TABLE [dbo].[SubLinhaIdioma] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_SubLinhaIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdSublinha]       UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma]     INT              NOT NULL,
    [NomeSubLinha]     VARCHAR (50)     NULL,
    [TextoUrl]         VARCHAR (200)    NULL,
    [TextoBotao]       VARCHAR (200)    NULL,
    [TextoInformativo] VARCHAR (200)    NULL,
    CONSTRAINT [PK_SubLinhaIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

