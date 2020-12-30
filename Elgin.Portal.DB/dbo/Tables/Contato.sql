CREATE TABLE [dbo].[Contato] (
    [Id]                 UNIQUEIDENTIFIER CONSTRAINT [DF_Contato_Id] DEFAULT (newid()) NOT NULL,
    [Data]               DATETIME         CONSTRAINT [DF_Contato_Data] DEFAULT (getdate()) NOT NULL,
    [Nome]               VARCHAR (250)    NOT NULL,
    [CpfCnpj]            VARCHAR (20)     NULL,
    [Email]              VARCHAR (250)    NOT NULL,
    [Telefone]           VARCHAR (20)     NOT NULL,
    [Tipo]               VARCHAR (50)     NULL,
    [IdLinha]            UNIQUEIDENTIFIER NULL,
    [Mensagem]           VARCHAR (800)    NOT NULL,
    [Assunto]            VARCHAR (250)    NULL,
    [EmpresaAreaAtuacao] VARCHAR (250)    NULL,
    CONSTRAINT [PK_Contato] PRIMARY KEY CLUSTERED ([Id] ASC)
);



