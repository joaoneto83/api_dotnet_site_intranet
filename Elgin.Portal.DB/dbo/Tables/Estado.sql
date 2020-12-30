CREATE TABLE [dbo].[Estado] (
    [Id]          INT             NOT NULL,
    [Descricao]   VARCHAR (100)   NOT NULL,
    [Sigla]       VARCHAR (2)     NOT NULL,
    [IdPais]      INT             NOT NULL,
    [TarifaSolar] DECIMAL (18, 2) CONSTRAINT [DF_Estado_TarifaSolar] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Estado] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Estado_Pais] FOREIGN KEY ([IdPais]) REFERENCES [dbo].[Pais] ([Id])
);



