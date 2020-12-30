CREATE TABLE [dbo].[TipoDependente] (
    [Id]                   UNIQUEIDENTIFIER NOT NULL,
    [NomeTipoDependente]   VARCHAR (30)     NULL,
    [CodigoTipoDependente] VARCHAR (20)     NULL,
    [Ativo]                BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_TipoDependente] PRIMARY KEY CLUSTERED ([Id] ASC)
);

