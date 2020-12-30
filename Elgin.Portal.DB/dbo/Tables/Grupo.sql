CREATE TABLE [dbo].[Grupo] (
    [Id]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [NomeGrupo]   VARCHAR (100)    NOT NULL,
    [Ativo]       BIT              CONSTRAINT [DF_Grupo_Ativo] DEFAULT ((1)) NULL,
    [DataCriacao] DATETIME         NULL,
    CONSTRAINT [PK_Grupo] PRIMARY KEY CLUSTERED ([Id] ASC)
);





