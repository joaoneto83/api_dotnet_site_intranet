CREATE TABLE [dbo].[AlternativaQuestaoAgendamentoUsuario] (
    [Id]                          UNIQUEIDENTIFIER CONSTRAINT [DF_AlternativaQuestaoAgendamentoUsuario_Id] DEFAULT (newid()) NOT NULL,
    [IdAlternativa]               UNIQUEIDENTIFIER NOT NULL,
    [IdQuestaoAgendamentoUsuario] UNIQUEIDENTIFIER NOT NULL,
    [Ordem]                       INT              NULL,
    [Selecionada]                 BIT              NULL,
    CONSTRAINT [PK_AlternativaQuestaoAgendamentoUsuario] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AlternativaQuestaoAgendamentoUsuario_Alternativa] FOREIGN KEY ([IdAlternativa]) REFERENCES [dbo].[Alternativa] ([Id]),
    CONSTRAINT [FK_AlternativaQuestaoAgendamentoUsuario_QuestaoAgendamentoUsuario] FOREIGN KEY ([IdQuestaoAgendamentoUsuario]) REFERENCES [dbo].[QuestaoAgendamentoUsuario] ([Id])
);

