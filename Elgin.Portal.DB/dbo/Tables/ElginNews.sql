CREATE TABLE [dbo].[ElginNews] (
    [Id]      INT           NOT NULL,
    [Nome]    VARCHAR (100) NOT NULL,
    [Ano]     INT           NOT NULL,
    [Numero]  INT           NOT NULL,
    [Imagem]  VARCHAR (200) NOT NULL,
    [Arquivo] VARCHAR (200) NOT NULL,
    [Data]    DATETIME      NOT NULL,
    [Ativo]   BIT           CONSTRAINT [DF_ElginNews_Ativo] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ElginNews] PRIMARY KEY CLUSTERED ([Id] ASC)
);





