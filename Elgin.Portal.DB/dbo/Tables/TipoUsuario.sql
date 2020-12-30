CREATE TABLE [dbo].[TipoUsuario] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [NomeTipoUsuario]   VARCHAR (50)     NULL,
    [CodigoTipoUsuario] VARCHAR (50)     NULL,
    [Ativo]             BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_TipoUsuario] PRIMARY KEY CLUSTERED ([Id] ASC)
);

