CREATE TABLE [dbo].[QuestaoAgendamentoUsuario] (
    [Id]                   UNIQUEIDENTIFIER CONSTRAINT [DF_QuestaoAgendamentoUsuario_Id] DEFAULT (newid()) NOT NULL,
    [IdQuestao]            UNIQUEIDENTIFIER NOT NULL,
    [IdAgendamentoUsuario] UNIQUEIDENTIFIER NOT NULL,
    [Ordem]                INT              NULL,
    CONSTRAINT [PK_QuestaoAgendamentoUsuario] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_QuestaoAgendamentoUsuario_AgendamentoUsuario] FOREIGN KEY ([IdAgendamentoUsuario]) REFERENCES [dbo].[AgendamentoUsuario] ([Id]),
    CONSTRAINT [FK_QuestaoAgendamentoUsuario_Questai] FOREIGN KEY ([IdQuestao]) REFERENCES [dbo].[Questao] ([Id])
);

