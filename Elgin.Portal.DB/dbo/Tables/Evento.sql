CREATE TABLE [dbo].[Evento] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [Titulo]    VARCHAR (150)    NOT NULL,
    [DataDe]    DATETIME         NOT NULL,
    [DataAte]   DATETIME         NOT NULL,
    [Descricao] VARCHAR (500)    NOT NULL,
    [Ativo]     BIT              NOT NULL,
    CONSTRAINT [PK_Evento] PRIMARY KEY CLUSTERED ([Id] ASC)
);

