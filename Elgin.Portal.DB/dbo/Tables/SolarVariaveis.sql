CREATE TABLE [dbo].[SolarVariaveis] (
    [PerdaSistema] DECIMAL (18, 5) NOT NULL,
    [PerdaPlaca]   DECIMAL (18, 5) NOT NULL,
    [MediaConsumo] DECIMAL (18, 5) CONSTRAINT [DF_SolarVariaveis_MediaConsumo] DEFAULT ((0)) NOT NULL
);



