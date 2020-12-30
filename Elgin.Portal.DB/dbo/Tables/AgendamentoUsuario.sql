CREATE TABLE [dbo].[AgendamentoUsuario] (
    [IdAgendamento] UNIQUEIDENTIFIER NOT NULL,
    [IdUsuario]     UNIQUEIDENTIFIER NOT NULL,
    [InicioProva]   DATETIME         NULL,
    [TerminoProva]  DATETIME         NULL,
    [Nota]          DECIMAL (5, 2)   NULL,
    [IdProva]       UNIQUEIDENTIFIER NOT NULL,
    [Id]            UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [TempoRestante] INT              NULL,
    [QtdAcertos]    INT              NULL,
    CONSTRAINT [PK_AgendamentoUsuario] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AgendamentoUsuario_Agendamento] FOREIGN KEY ([IdAgendamento]) REFERENCES [dbo].[Agendamento] ([Id]),
    CONSTRAINT [FK_AgendamentoUsuario_Prova] FOREIGN KEY ([IdProva]) REFERENCES [dbo].[Prova] ([Id]),
    CONSTRAINT [FK_AgendamentoUsuario_Usuario] FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[Usuario] ([Id])
);





