CREATE TABLE [dbo].[SecaoModelo] (
    [Id]                 UNIQUEIDENTIFIER CONSTRAINT [DF_SecaoModelo_Id] DEFAULT (newid()) NOT NULL,
    [IdSecao]            UNIQUEIDENTIFIER NOT NULL,
    [IdSecaoModeloGrupo] UNIQUEIDENTIFIER NOT NULL,
    [Texto1]             VARCHAR (200)    NULL,
    [Texto2]             VARCHAR (500)    NULL,
    [Texto3]             VARCHAR (4000)   NULL,
    [CodigoVideo]        VARCHAR (50)     NULL,
    [AparelhoIdeal]      BIT              CONSTRAINT [DF_SecaoModelo_AparelhoIdeal] DEFAULT ((0)) NOT NULL,
    [Ordem]              INT              CONSTRAINT [DF_SecaoModelo_Ordem] DEFAULT ((0)) NOT NULL,
    [Ativo]              BIT              CONSTRAINT [DF_SecaoModelo_Ativo] DEFAULT ((1)) NOT NULL,
    [CodigoVideo2]       VARCHAR (50)     NULL,
    [CodigoVideo3]       VARCHAR (50)     NULL,
    [CodigoVideo4]       VARCHAR (50)     NULL,
    CONSTRAINT [PK_SecaoModelo] PRIMARY KEY CLUSTERED ([Id] ASC)
);

