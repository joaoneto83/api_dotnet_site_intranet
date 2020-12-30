CREATE TABLE [dbo].[SecaoModeloIcone] (
    [Id]                UNIQUEIDENTIFIER CONSTRAINT [DF_SecaoModeloIcone_Id] DEFAULT (newid()) NOT NULL,
    [DescricaoIcone]    VARCHAR (150)    NULL,
    [IdSecaoModelo]     UNIQUEIDENTIFIER NULL,
    [IdIcone]           UNIQUEIDENTIFIER NULL,
    [Ordem]             INT              NULL,
    [SubDescricaoIcone] VARCHAR (150)    NULL,
    CONSTRAINT [PK_SecaoModeloIcone] PRIMARY KEY CLUSTERED ([Id] ASC)
);

