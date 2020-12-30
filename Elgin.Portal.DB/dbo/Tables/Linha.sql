CREATE TABLE [dbo].[Linha] (
    [Id]           UNIQUEIDENTIFIER NOT NULL,
    [CodigoLinha]  VARCHAR (50)     NULL,
    [CodigoLegado] VARCHAR (50)     NULL,
    [NomeLinha]    VARCHAR (50)     NULL,
    [Ativo]        BIT              NULL,
    [Cor1]         VARCHAR (10)     NULL,
    [Cor2]         VARCHAR (10)     NULL,
    [CorTitulo]    VARCHAR (10)     NULL,
    [Ordem]        INT              NULL,
    CONSTRAINT [PK_Linha] PRIMARY KEY CLUSTERED ([Id] ASC)
);

