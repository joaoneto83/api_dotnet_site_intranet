CREATE TABLE [dbo].[Integrador] (
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    [Nome]          VARCHAR (250)    NOT NULL,
    [Endereco]      VARCHAR (500)    NULL,
    [TelefoneFixo]  VARCHAR (100)    NULL,
    [TelefoneMovel] VARCHAR (100)    NULL,
    [Email]         VARCHAR (250)    NULL,
    [Site]          VARCHAR (100)    NULL,
    [IdPais]        INT              NOT NULL,
    [UF]            VARCHAR (2)      NOT NULL,
    [IdCidade]      INT              NULL,
    [Ativo]         BIT              NOT NULL,
    [IdSegmento]    INT              NOT NULL,
    CONSTRAINT [PK_Integrador] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Integrador_Cidade] FOREIGN KEY ([IdCidade]) REFERENCES [dbo].[Cidade] ([Id]),
    CONSTRAINT [FK_Integrador_Pais] FOREIGN KEY ([IdPais]) REFERENCES [dbo].[Pais] ([Id]),
    CONSTRAINT [FK_Integrador_Segmento] FOREIGN KEY ([IdSegmento]) REFERENCES [dbo].[Segmento] ([Id])
);

