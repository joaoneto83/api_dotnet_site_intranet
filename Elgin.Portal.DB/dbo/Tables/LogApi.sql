CREATE TABLE [dbo].[LogApi] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [EventId]     INT          NULL,
    [LogLevel]    VARCHAR (50) NOT NULL,
    [Message]     NTEXT        NOT NULL,
    [CreatedTime] DATETIME     NOT NULL,
    CONSTRAINT [PK_LogApi] PRIMARY KEY CLUSTERED ([Id] ASC)
);

