CREATE TABLE [dbo].[Secao] (
    [Id]                 UNIQUEIDENTIFIER CONSTRAINT [DF_Secao_Id] DEFAULT (newid()) NOT NULL,
    [Nome]               VARCHAR (50)     NOT NULL,
    [Codigo]             VARCHAR (50)     NOT NULL,
    [Descricao]          VARCHAR (200)    NOT NULL,
    [QtdImagens]         INT              CONSTRAINT [DF_Secao_QtdImagens] DEFAULT ((0)) NOT NULL,
    [ExibeTexto1]        BIT              CONSTRAINT [DF_Secao_ExibeTexto1] DEFAULT ((1)) NOT NULL,
    [ExibeTexto2]        BIT              CONSTRAINT [DF_Secao_ExibeTexto2] DEFAULT ((1)) NOT NULL,
    [ExibeTexto3]        BIT              CONSTRAINT [DF_Secao_ExibeTexto3] DEFAULT ((1)) NOT NULL,
    [ExibeCodigoVideo]   BIT              CONSTRAINT [DF_Secao_ExibeUrlVideo] DEFAULT ((0)) NOT NULL,
    [ExibeCodigoVideo2]  BIT              CONSTRAINT [DF_Secao_ExibeCodigoVideo1] DEFAULT ((0)) NOT NULL,
    [ExibeCodigoVideo3]  BIT              CONSTRAINT [DF_Secao_ExibeCodigoVideo1_1] DEFAULT ((0)) NOT NULL,
    [ExibeCodigoVideo4]  BIT              CONSTRAINT [DF_Secao_ExibeCodigoVideo1_2] DEFAULT ((0)) NOT NULL,
    [ExibeAparelhoIdeal] BIT              CONSTRAINT [DF_Secao_ExibeAparelhoIdeal] DEFAULT ((0)) NOT NULL,
    [QtdIcones]          INT              NULL,
    [QtdlVideos]         INT              NULL,
    [Qltvideo]           INT              NULL,
    CONSTRAINT [PK_Secao] PRIMARY KEY CLUSTERED ([Id] ASC)
);



