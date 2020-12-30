CREATE TABLE [dbo].[ProdutoEspecificacaoTecnicaIdioma] (
    [Id]                            UNIQUEIDENTIFIER CONSTRAINT [DF_ProdutoEspecificacaoTecnicaIdioma_Id] DEFAULT (newid()) NOT NULL,
    [IdProdutoEspecificacaoTecnica] VARCHAR (200)    NOT NULL,
    [CodigoIdioma]                  INT              NOT NULL,
    [Valor]                         VARCHAR (100)    NULL,
    CONSTRAINT [PK_ProdutoEspecificacaoTecnicaIdioma] PRIMARY KEY CLUSTERED ([Id] ASC)
);

