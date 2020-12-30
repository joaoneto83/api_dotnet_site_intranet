CREATE TABLE [dbo].[PerguntaFrequente] (
    [Id]               UNIQUEIDENTIFIER NOT NULL,
    [Pergunta]         VARCHAR (MAX)    NULL,
    [Resposta]         VARCHAR (MAX)    NULL,
    [CodigoComponente] VARCHAR (50)     NULL,
    [Ordem]            INT              NULL,
    CONSTRAINT [PK_PerguntasFrequentes] PRIMARY KEY CLUSTERED ([Id] ASC)
);

