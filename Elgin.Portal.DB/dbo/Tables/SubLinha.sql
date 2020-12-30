CREATE TABLE [dbo].[SubLinha] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [CodigoLegado]        VARCHAR (50)     NULL,
    [CodigoSubLinha]      VARCHAR (50)     NULL,
    [NomeSubLinha]        VARCHAR (50)     NULL,
    [IdLinha]             UNIQUEIDENTIFIER NULL,
    [Ativo]               BIT              NULL,
    [IdArquivo]           UNIQUEIDENTIFIER NULL,
    [Ordem]               INT              CONSTRAINT [DF_SubLinha_Ordem] DEFAULT ((0)) NOT NULL,
    [MostraAparelhoIdeal] BIT              CONSTRAINT [DF_SubLinha_MostraAparelhoIdeal] DEFAULT ((0)) NOT NULL,
    [PossuiFiltroPilha]   BIT              CONSTRAINT [DF_SubLinha_PossuiFiltroPilha] DEFAULT ((0)) NOT NULL,
    [MostraLink]          BIT              NULL,
    [MostraRota]          BIT              NULL,
    [TextoUrl]            VARCHAR (200)    NULL,
    [TextoBotao]          VARCHAR (200)    NULL,
    [TextoInformativo]    VARCHAR (200)    NULL,
    CONSTRAINT [PK_Categoria] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Categoria_Linha] FOREIGN KEY ([IdLinha]) REFERENCES [dbo].[Linha] ([Id])
);







