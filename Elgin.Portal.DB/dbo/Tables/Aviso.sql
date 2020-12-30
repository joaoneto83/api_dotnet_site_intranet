CREATE TABLE [dbo].[Aviso] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [IdTipoAviso] UNIQUEIDENTIFIER NOT NULL,
    [Modulo]      VARCHAR (50)     NOT NULL,
    [Titulo]      VARCHAR (150)    NOT NULL,
    [Descricao]   VARCHAR (500)    NOT NULL,
    [DataAviso]   DATETIME         NOT NULL,
    [Ativo]       BIT              NOT NULL,
    CONSTRAINT [PK_Aviso] PRIMARY KEY CLUSTERED ([Id] ASC)
);



