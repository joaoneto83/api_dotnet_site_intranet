CREATE TABLE [dbo].[GrupoDestaque] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [NomeGrupoDestaque]   VARCHAR (100)    NULL,
    [CodigoGrupoDestaque] VARCHAR (50)     NULL,
    [Ativo]               BIT              DEFAULT ((1)) NOT NULL,
    [Modulo]              VARCHAR (50)     NULL,
    [Ordem]               INT              NULL,
    [Link]                VARCHAR (200)    NULL,
    CONSTRAINT [PK_GrupoDestaque] PRIMARY KEY CLUSTERED ([Id] ASC)
);

