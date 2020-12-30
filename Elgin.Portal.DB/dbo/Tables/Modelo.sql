CREATE TABLE [dbo].[Modelo] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_Modelo_Id] DEFAULT (newid()) NOT NULL,
    [CodigoModelo]     VARCHAR (50)     NULL,
    [NomeModelo]       VARCHAR (100)    NULL,
    [IdProduto]        UNIQUEIDENTIFIER NULL,
    [AparelhoIdealDe]  DECIMAL (18, 2)  NULL,
    [AparelhoIdealAte] DECIMAL (18, 2)  NULL,
    [Ativo]            BIT              NULL,
    CONSTRAINT [PK_Modelo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Modelo_Produto] FOREIGN KEY ([IdProduto]) REFERENCES [dbo].[Produto] ([Id])
);

