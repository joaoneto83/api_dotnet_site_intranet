CREATE TABLE [dbo].[AssistenciaVinculadaSubLinha] (
    [IdSubLinha]             UNIQUEIDENTIFIER NULL,
    [IdAssistenciaVinculada] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [FK_AssistenciaVinculadaSubLinha_AssistenciaVinculada] FOREIGN KEY ([IdAssistenciaVinculada]) REFERENCES [dbo].[AssistenciaVinculada] ([Id]),
    CONSTRAINT [FK_AssistenciaVinculadaSubLinha_SubLinha] FOREIGN KEY ([IdSubLinha]) REFERENCES [dbo].[SubLinha] ([Id])
);

