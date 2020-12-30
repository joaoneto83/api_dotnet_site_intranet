CREATE TABLE [dbo].[AssistenciaVinculada] (
    [Id]        UNIQUEIDENTIFIER NOT NULL,
    [Documento] VARCHAR (20)     NOT NULL,
    [Ativo]     BIT              NULL,
    CONSTRAINT [PK_AssistenciaVinculada] PRIMARY KEY CLUSTERED ([Id] ASC)
);

