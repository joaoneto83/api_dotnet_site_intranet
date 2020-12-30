CREATE TABLE [dbo].[Segmento] (
    [Id]     INT          IDENTITY (1, 1) NOT NULL,
    [Nome]   VARCHAR (50) NOT NULL,
    [Codigo] VARCHAR (50) NOT NULL,
    [Ordem]  INT          NOT NULL,
    [Ativo]  BIT          NOT NULL,
    CONSTRAINT [PK_Segmento] PRIMARY KEY CLUSTERED ([Id] ASC)
);

