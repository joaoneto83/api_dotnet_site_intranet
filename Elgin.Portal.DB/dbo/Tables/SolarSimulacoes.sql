CREATE TABLE [dbo].[SolarSimulacoes] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_SolarSimulacoes_Id] DEFAULT (newid()) NOT NULL,
    [Nome]              VARCHAR (200)    NOT NULL,
    [Email]             VARCHAR (200)    NOT NULL,
    [Estado]            VARCHAR (200)    NOT NULL,
    [Cidade]            VARCHAR (200)    NOT NULL,
    [IdCidade]          INT              NOT NULL,
    [UF]                VARCHAR (2)      NOT NULL,
    [Onde]              VARCHAR (10)     NOT NULL,
    [FormatoFatura]     VARCHAR (10)     NOT NULL,
    [FaturaMensal]      VARCHAR (50)     NOT NULL,
    [CondicaoSolar]     VARCHAR (50)     NOT NULL,
    [EstimativaArea]    VARCHAR (50)     NOT NULL,
    [Economia]          VARCHAR (50)     NOT NULL,
    [EcominaFinanceira] VARCHAR (50)     NOT NULL,
    [Kit1]              VARCHAR (50)     NOT NULL,
    [Kit2]              VARCHAR (50)     NOT NULL,
    [KitTotal]          VARCHAR (50)     NOT NULL,
    [KitNecessario]     VARCHAR (50)     NOT NULL,
    [Data]              DATETIME         CONSTRAINT [DF_SolarSimulacoes_Data] DEFAULT (getdate()) NOT NULL,
    [MediaMensal] VARCHAR(50) NULL, 
    CONSTRAINT [PK_SolarSimulacoes] PRIMARY KEY CLUSTERED ([Id] ASC)
);



