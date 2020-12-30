CREATE TABLE [dbo].[Menu] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_Menu_Id] DEFAULT (newid()) NOT NULL,
    [Nome]      VARCHAR (50)     NOT NULL,
    [Codigo]    VARCHAR (50)     CONSTRAINT [DF_Menu_Codigo] DEFAULT ('teste') NOT NULL,
    [Rota]      VARCHAR (50)     NOT NULL,
    [Ordem]     INT              CONSTRAINT [DF_Menu_Ordem] DEFAULT ((0)) NOT NULL,
    [IdMenuPai] UNIQUEIDENTIFIER NULL,
    [Ativo]     BIT              CONSTRAINT [DF_Menu_Ativo] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED ([Id] ASC)
);

