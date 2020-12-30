CREATE TABLE [dbo].[Assistencia] (
    [Id]          INT           NOT NULL,
    [Nome]        VARCHAR (255) DEFAULT (NULL) NULL,
    [Rua]         VARCHAR (255) DEFAULT (NULL) NULL,
    [Numero]      VARCHAR (255) DEFAULT (NULL) NULL,
    [Bairro]      VARCHAR (255) DEFAULT (NULL) NULL,
    [Complemento] VARCHAR (255) DEFAULT (NULL) NULL,
    [Cep]         VARCHAR (255) DEFAULT (NULL) NULL,
    [Telefone]    VARCHAR (255) DEFAULT (NULL) NULL,
    [Telefone_2]  VARCHAR (255) DEFAULT (NULL) NULL,
    [Telefone_3]  VARCHAR (255) DEFAULT (NULL) NULL,
    [Telefone_4]  VARCHAR (255) DEFAULT (NULL) NULL,
    [Email]       VARCHAR (255) DEFAULT (NULL) NULL,
    [IdCidade]    INT           DEFAULT (NULL) NULL,
    [IdEstado]    INT           DEFAULT (NULL) NULL,
    [Lat]         VARCHAR (255) DEFAULT (NULL) NULL,
    [Long]        VARCHAR (255) DEFAULT (NULL) NULL
);

