CREATE TABLE [dbo].[LogEmail] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [Destinatarios] VARCHAR (MAX)    NOT NULL,
    [Assunto]       VARCHAR (100)    NOT NULL,
    [DataEnvio]     DATETIME         NOT NULL
);

