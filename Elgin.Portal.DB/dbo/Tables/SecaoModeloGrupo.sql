CREATE TABLE [dbo].[SecaoModeloGrupo] (
    [Id]        UNIQUEIDENTIFIER CONSTRAINT [DF_SecaoModeloGrupo_Id] DEFAULT (newid()) NOT NULL,
    [Descricao] VARCHAR (50)     NOT NULL,
    [Ativo]     BIT              CONSTRAINT [DF_SecaoModeloGrupo_Ativo] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_SecaoModeloGrupo] PRIMARY KEY CLUSTERED ([Id] ASC)
);

