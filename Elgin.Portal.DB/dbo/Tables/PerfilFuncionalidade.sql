CREATE TABLE [dbo].[PerfilFuncionalidade] (
    [IdPerfil]         UNIQUEIDENTIFIER NOT NULL,
    [IdFuncionalidade] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_PerfilFuncionalidade] PRIMARY KEY CLUSTERED ([IdPerfil] ASC, [IdFuncionalidade] ASC)
);

