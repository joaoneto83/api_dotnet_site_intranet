CREATE TABLE [dbo].[Arquivo] (
    [Id]            UNIQUEIDENTIFIER CONSTRAINT [DF_Arquivo_Id] DEFAULT (newid()) NOT NULL,
    [IdTipoArquivo] UNIQUEIDENTIFIER NULL,
    [NomeArquivo]   VARCHAR (150)    NULL,
    [Caminho]       VARCHAR (500)    NULL,
    [Ordem]         INT              CONSTRAINT [DF_Arquivo_Ordem] DEFAULT ((0)) NOT NULL,
    [Ativo]         BIT              NULL,
    [IdPai]         UNIQUEIDENTIFIER NULL,
    [IdSecao]       UNIQUEIDENTIFIER NULL,
    [Linque]       VARCHAR (500)    NULL,
    CONSTRAINT [PK_Arquivo] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Arquivo_TipoArquivo] FOREIGN KEY ([IdTipoArquivo]) REFERENCES [dbo].[TipoArquivo] ([Id])
);

