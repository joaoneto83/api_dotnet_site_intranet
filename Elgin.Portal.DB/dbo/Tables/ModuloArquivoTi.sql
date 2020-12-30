CREATE TABLE [dbo].[ModuloArquivoTi] (
    [Id]    UNIQUEIDENTIFIER NOT NULL,
    [Nome]  VARCHAR (50)     NOT NULL,
    [Secao] VARCHAR (50)     NOT NULL,
    [Ativo] BIT              NOT NULL,
    CONSTRAINT [PK_SecaoTi] PRIMARY KEY CLUSTERED ([Id] ASC)
);

