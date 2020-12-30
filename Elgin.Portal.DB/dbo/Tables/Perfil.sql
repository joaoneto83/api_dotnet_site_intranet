CREATE TABLE [dbo].[Perfil] (
    [Id]    UNIQUEIDENTIFIER CONSTRAINT [DF_Perfil_Id] DEFAULT (newid()) NOT NULL,
    [Nome]  VARCHAR (50)     NOT NULL,
    [Ativo] BIT              CONSTRAINT [DF_Perfil_Ativo] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Perfil] PRIMARY KEY CLUSTERED ([Id] ASC)
);

