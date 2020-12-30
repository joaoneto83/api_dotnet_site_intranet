CREATE TABLE [dbo].[EventoRefrigeracaoIdioma] (
    [Id]                   UNIQUEIDENTIFIER CONSTRAINT [DF_EventoRefrigeracaoIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdEventoRefrigeracao] UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma]         INT              NOT NULL,
    [Titulo]               VARCHAR (150)    NOT NULL,
    CONSTRAINT [PK_EventoRefrigeracaoIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

