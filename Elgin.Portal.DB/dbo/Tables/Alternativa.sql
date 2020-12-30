CREATE TABLE [dbo].[Alternativa] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [IdQuestao] UNIQUEIDENTIFIER NOT NULL,
    [Descricao] VARCHAR (150)    NULL,
    [Correta]   BIT              NULL,
    [Ativo]     BIT              NULL,
    CONSTRAINT [PK_Alternativa] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Alternativa_Questao] FOREIGN KEY ([IdQuestao]) REFERENCES [dbo].[Questao] ([Id])
);

