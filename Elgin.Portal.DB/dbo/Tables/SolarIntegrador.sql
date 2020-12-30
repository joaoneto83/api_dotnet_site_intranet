CREATE TABLE [dbo].[SolarIntegrador] (
    [Id]       UNIQUEIDENTIFIER CONSTRAINT [DF_SolarIntegrador_Id] DEFAULT (newid()) NOT NULL,
    [UF]       VARCHAR (2)      NOT NULL,
    [Nome]     VARCHAR (250)    NOT NULL,
    [CNPJ]     VARCHAR (20)     NOT NULL,
    [Endereco] VARCHAR (500)    NULL,
    [Email]    VARCHAR (250)    NULL,
    [Telefone] VARCHAR (100)    NULL,
    [Site]     VARCHAR (100)    NULL,
    [IdCidade] INT              NULL,
    [Ativo]    BIT              CONSTRAINT [DF_SolarIntegrador_Ativo] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_SolarIntegrador] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SolarIntegrador_Cidade] FOREIGN KEY ([IdCidade]) REFERENCES [dbo].[Cidade] ([Id])
);







