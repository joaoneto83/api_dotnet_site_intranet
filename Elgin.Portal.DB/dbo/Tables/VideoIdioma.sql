CREATE TABLE [dbo].[VideoIdioma] (
    [Id]             UNIQUEIDENTIFIER CONSTRAINT [DF_VideoIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdVideo]        UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma]   INT              NOT NULL,
    [LinkVideo]      VARCHAR (250)    NULL,
    [TituloVideo]    VARCHAR (150)    NULL,
    [DescricaoVideo] VARCHAR (250)    NULL,
    CONSTRAINT [PK_VideoIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

