CREATE TABLE [dbo].[Pais] (
    [Id]    INT          NOT NULL,
    [Nome]  VARCHAR (60) NOT NULL,
    [Sigla] VARCHAR (10) NOT NULL,
    CONSTRAINT [PK_Pais] PRIMARY KEY CLUSTERED ([Id] ASC)
);

