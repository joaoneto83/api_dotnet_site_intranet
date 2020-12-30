CREATE TABLE [dbo].[SecaoProduto] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_SecaoProduto_Id] DEFAULT (newid()) NOT NULL,
    [IdSecao]       UNIQUEIDENTIFIER NOT NULL,
    [IdProduto]     UNIQUEIDENTIFIER NOT NULL,
    [Texto1]        VARCHAR (200)    NULL,
    [Texto2]        VARCHAR (500)    NULL,
    [Texto3]        VARCHAR (4000)   NULL,
    [CodigoVideo]   VARCHAR (50)     NULL,
    [AparelhoIdeal] BIT              CONSTRAINT [DF_SecaoProduto_AparelhoIdeal] DEFAULT ((0)) NOT NULL,
    [Ordem]         INT              CONSTRAINT [DF_SecaoProduto_Ordem] DEFAULT ((0)) NOT NULL,
    [Ativo]         BIT              CONSTRAINT [DF_SecaoProduto_Ativo] DEFAULT ((1)) NOT NULL,
    [IdSecaoModelo] UNIQUEIDENTIFIER NULL,
    [CodigoVideo2]  VARCHAR (50)     NULL,
    [CodigoVideo3]  VARCHAR (50)     NULL,
    [CodigoVideo4]  VARCHAR (50)     NULL,
    CONSTRAINT [PK_SecaoProduto] PRIMARY KEY CLUSTERED ([Id] ASC)
);





