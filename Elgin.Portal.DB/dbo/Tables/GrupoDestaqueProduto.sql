CREATE TABLE [dbo].[GrupoDestaqueProduto] (
    [IdGrupoDestaque] UNIQUEIDENTIFIER NOT NULL,
    [IdProduto]       UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_GrupoDestaqueProduto_GrupoDestaque] FOREIGN KEY ([IdGrupoDestaque]) REFERENCES [dbo].[GrupoDestaque] ([Id]),
    CONSTRAINT [FK_GrupoDestaqueProduto_Produto] FOREIGN KEY ([IdProduto]) REFERENCES [dbo].[Produto] ([Id])
);

