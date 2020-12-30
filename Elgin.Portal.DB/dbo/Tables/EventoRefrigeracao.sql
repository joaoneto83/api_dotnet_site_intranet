CREATE TABLE [dbo].[EventoRefrigeracao] (
    [Id]                       UNIQUEIDENTIFIER NOT NULL,
    [Titulo]                   VARCHAR (150)    NOT NULL,
    [DataDe]                   DATETIME         NOT NULL,
    [DataAte]                  DATETIME         NULL,
    [Local]                    VARCHAR (500)    NOT NULL,
    [CaminhoImagem]            VARCHAR (200)    NULL,
    [IdTipoEventoRefrigeracao] INT              NOT NULL,
    [Link]                     VARCHAR (150)    NULL,
    CONSTRAINT [PK_EventoRefrigeracao] PRIMARY KEY CLUSTERED ([Id] ASC)
);

