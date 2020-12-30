CREATE TABLE [dbo].[GrupoUsuario] (
    [IdGrupo]   UNIQUEIDENTIFIER NULL,
    [IdUsuario] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_GrupoUsuario_Grupo] FOREIGN KEY ([IdGrupo]) REFERENCES [dbo].[Grupo] ([Id]),
    CONSTRAINT [PK_GrupoUsuario_Usuario] FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[Usuario] ([Id])
);





