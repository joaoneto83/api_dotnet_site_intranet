CREATE TABLE [dbo].[CaracteristicaIdioma] (
    [Id]                      UNIQUEIDENTIFIER CONSTRAINT [DF_CaracteristicaIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdCaracteristica]        UNIQUEIDENTIFIER NOT NULL,
    [CodigoIdioma]            INT              NOT NULL,
    [DescricaoCaracteristica] VARCHAR (150)    NULL,
    CONSTRAINT [PK_CaracteristicaIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

