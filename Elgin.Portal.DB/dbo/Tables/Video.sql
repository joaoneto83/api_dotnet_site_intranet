CREATE TABLE [dbo].[Video] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [LinkVideo]      VARCHAR (250)    NULL,
    [TituloVideo]    VARCHAR (150)    NULL,
    [DescricaoVideo] VARCHAR (250)    NULL,
    [Modulo]         VARCHAR (50)     NOT NULL,
    [Ativo]          BIT              DEFAULT ((1)) NOT NULL,
    [Ordem]          INT              NULL,
    CONSTRAINT [PK_Video] PRIMARY KEY CLUSTERED ([Id] ASC)
);

