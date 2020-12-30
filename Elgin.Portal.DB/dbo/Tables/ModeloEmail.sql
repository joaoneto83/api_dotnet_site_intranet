CREATE TABLE [dbo].[ModeloEmail] (
    [Id]     UNIQUEIDENTIFIER CONSTRAINT [DF_ModeloEmail_Id] DEFAULT (newid()) NOT NULL,
    [Codigo] VARCHAR (50)     NOT NULL,
    [Html]   VARCHAR (MAX)    NOT NULL,
    CONSTRAINT [PK_ModeloEmail] PRIMARY KEY CLUSTERED ([Id] ASC)
);

