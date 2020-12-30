CREATE TABLE [dbo].[Setor] (
    [Id]          UNIQUEIDENTIFIER CONSTRAINT [DF_Setor_Id] DEFAULT (newid()) NOT NULL,
    [NomeSetor]   VARCHAR (150)    NOT NULL,
    [CodigoSetor] VARCHAR (50)     NULL,
    [Ativo]       BIT              NULL,
    CONSTRAINT [PK_Setor] PRIMARY KEY CLUSTERED ([Id] ASC)
);



