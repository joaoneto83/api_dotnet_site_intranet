CREATE TABLE [dbo].[TipoEventoRefrigeracao] (
    [Id]     INT          NOT NULL,
    [Nome]   VARCHAR (50) NOT NULL,
    [Codigo] VARCHAR (50) NOT NULL,
    [Ordem]  INT          NOT NULL,
    [Ativo]  BIT          NOT NULL,
    CONSTRAINT [PK_TipoEventoRefrigeracao_1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

