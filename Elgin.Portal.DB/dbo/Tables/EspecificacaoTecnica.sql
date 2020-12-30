CREATE TABLE [dbo].[EspecificacaoTecnica] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [CodigoEspecificacao] VARCHAR (50)     NULL,
    [NomeEspecificacao]   VARCHAR (100)    NULL,
    [IdSubLinha]          UNIQUEIDENTIFIER NULL,
    [Ativo]               BIT              NULL,
    [Comparativo]         BIT              CONSTRAINT [DF_EspecificacaoTecnica_Comparativo] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_EspecificacaoTecnica] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EspecificacaoTecnica_Categoria] FOREIGN KEY ([IdSubLinha]) REFERENCES [dbo].[SubLinha] ([Id])
);



