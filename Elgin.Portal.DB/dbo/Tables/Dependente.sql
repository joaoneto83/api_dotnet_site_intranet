CREATE TABLE [dbo].[Dependente] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [NomeDependente]   VARCHAR (100)    NULL,
    [Celular]          VARCHAR (20)     NULL,
    [DataNascimento]   DATETIME         NULL,
    [IdUsuario]        UNIQUEIDENTIFIER NOT NULL,
    [IdTipoDependente] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Dependente] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Dependente_TipoDependente] FOREIGN KEY ([IdTipoDependente]) REFERENCES [dbo].[TipoDependente] ([Id]),
    CONSTRAINT [FK_Dependente_Usuario] FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[Usuario] ([Id])
);

