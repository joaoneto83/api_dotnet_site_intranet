CREATE TABLE [dbo].[AgendaTreinamentos] (
    [Id]      UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [DataDe]  DATETIME         NULL,
    [DataAte] DATETIME         NULL,
    [Empresa] VARCHAR (100)    NULL,
    [Cidade]  VARCHAR (100)    NULL,
    [Estado]  VARCHAR (2)      NULL,
    CONSTRAINT [PK_AgendaTreinamentos] PRIMARY KEY CLUSTERED ([Id] ASC)
);

