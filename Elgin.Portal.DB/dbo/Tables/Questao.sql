CREATE TABLE [dbo].[Questao] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [IdProva]   UNIQUEIDENTIFIER NOT NULL,
    [Descricao] VARCHAR (150)    NULL,
    [Ativo]     BIT              NULL,
    CONSTRAINT [PK_Questao] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Questao_Prova] FOREIGN KEY ([IdProva]) REFERENCES [dbo].[Prova] ([Id])
);

