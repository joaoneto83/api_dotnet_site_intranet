CREATE TABLE [dbo].[TipoAviso] (
    [Id]              UNIQUEIDENTIFIER NOT NULL,
    [NomeTipoAviso]   VARCHAR (50)     NOT NULL,
    [CodigoTipoAviso] VARCHAR (50)     NOT NULL,
    [Ativo]           BIT              NULL,
    CONSTRAINT [PK_TipoAviso] PRIMARY KEY CLUSTERED ([Id] ASC)
);

