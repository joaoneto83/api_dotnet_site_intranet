CREATE TABLE [dbo].[BannerIdioma] (
    [Id]           UNIQUEIDENTIFIER CONSTRAINT [DF_BannerIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdBanner]     UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma] INT              NOT NULL,
    [texto1]       VARCHAR (100)    NULL,
    [texto2]       VARCHAR (100)    NULL,
    [texto3]       VARCHAR (100)    NULL,
    CONSTRAINT [PK_BannerIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

