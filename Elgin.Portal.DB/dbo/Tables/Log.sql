CREATE TABLE [dbo].[Log] (
    [Id]      UNIQUEIDENTIFIER CONSTRAINT [DF_Log_Id] DEFAULT (newid()) NOT NULL,
    [Data]    DATETIME         CONSTRAINT [DF_Log_Data] DEFAULT (getdate()) NOT NULL,
    [Message] NTEXT            NOT NULL,
    [Url]     VARCHAR (250)    NOT NULL,
    [Stack]   NTEXT            NOT NULL,
    CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED ([Id] ASC)
);

