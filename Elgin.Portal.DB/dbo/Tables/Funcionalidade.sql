CREATE TABLE [dbo].[Funcionalidade] (
    [Id]     UNIQUEIDENTIFIER CONSTRAINT [DF_Funcionalidade_Id] DEFAULT (newid()) NOT NULL,
    [Nome]   VARCHAR (100)    NOT NULL,
    [Codigo] VARCHAR (50)     NOT NULL,
    [IdMenu] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Funcionalidade] PRIMARY KEY CLUSTERED ([Id] ASC)
);

