CREATE TABLE [dbo].[Notificacao] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_Notificacao_Id] DEFAULT (newid()) NOT NULL,
    [IdUsuario]        UNIQUEIDENTIFIER NOT NULL,
    [Tipo]             VARCHAR (50)     NOT NULL,
    [Descricao]        VARCHAR (500)    NOT NULL,
    [Link]             VARCHAR (250)    NULL,
    [DataInclusao]     DATETIME         CONSTRAINT [DF_Notificacao_DataInclusao] DEFAULT (getdate()) NOT NULL,
    [DataVisualizacao] DATETIME         NULL,
    [DataExclusao]     DATETIME         NULL,
    CONSTRAINT [PK_Notificacao] PRIMARY KEY CLUSTERED ([Id] ASC)
);

