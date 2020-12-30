CREATE TABLE [dbo].[Agendamento] (
    [Id]                 UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [IdProva]            UNIQUEIDENTIFIER NOT NULL,
    [DataDe]             DATETIME         NOT NULL,
    [DataAte]            DATETIME         NOT NULL,
    [QtdQuestoes]        INT              NOT NULL,
    [Duracao]            INT              NOT NULL,
    [Ativo]              BIT              NOT NULL,
    [IdUsuarioAlteracao] UNIQUEIDENTIFIER NULL,
    [IdGrupo]            UNIQUEIDENTIFIER NULL,
    [IdPessoa]           UNIQUEIDENTIFIER NULL,
    [Descricao]          VARCHAR (150)    NOT NULL,
    CONSTRAINT [PK_Agendamento] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Agendamento_Grupo] FOREIGN KEY ([IdGrupo]) REFERENCES [dbo].[Grupo] ([Id]),
    CONSTRAINT [FK_Agendamento_Prova] FOREIGN KEY ([IdProva]) REFERENCES [dbo].[Prova] ([Id]),
    CONSTRAINT [FK_Agendamento_Usuario] FOREIGN KEY ([IdPessoa]) REFERENCES [dbo].[Usuario] ([Id])
);







