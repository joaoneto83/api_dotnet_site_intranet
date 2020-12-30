CREATE TABLE [dbo].[Cidade] (
    [Id]              INT           NOT NULL,
    [IdEstado]        INT           NOT NULL,
    [Descricao]       VARCHAR (100) NOT NULL,
    [Longitude]       FLOAT (53)    NULL,
    [Latitude]        FLOAT (53)    NULL,
    [SolarMediaAnual] FLOAT (53)    NULL,
    CONSTRAINT [PK_Cidade] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Cidade_Estado] FOREIGN KEY ([IdEstado]) REFERENCES [dbo].[Estado] ([Id])
);



