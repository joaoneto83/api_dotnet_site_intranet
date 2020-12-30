CREATE TABLE [dbo].[TipoArquivo] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_TipoArquivo_Id] DEFAULT (newid()) NOT NULL,
    [NomeTipoArquivo]   VARCHAR (50)     NULL,
    [CodigoTipoArquivo] VARCHAR (50)     NULL,
    [Ativo]             BIT              NULL,
    CONSTRAINT [PK_TipoArquivo] PRIMARY KEY CLUSTERED ([Id] ASC)
);

