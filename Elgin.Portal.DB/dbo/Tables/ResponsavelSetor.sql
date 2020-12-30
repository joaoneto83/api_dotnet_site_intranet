CREATE TABLE [dbo].[ResponsavelSetor] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Setor]    VARCHAR (150)    NOT NULL,
    [Nome]     VARCHAR (150)    NOT NULL,
    [Telefone] VARCHAR (50)     NOT NULL,
    [Ativo]    BIT              NOT NULL,
    CONSTRAINT [PK_ResponsavelSetor] PRIMARY KEY CLUSTERED ([Id] ASC)
);



