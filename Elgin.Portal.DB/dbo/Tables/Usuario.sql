CREATE TABLE [dbo].[Usuario] (
    [Id]                    UNIQUEIDENTIFIER CONSTRAINT [DF_Usuario_Id] DEFAULT (newid()) NOT NULL,
    [Nome]                  VARCHAR (100)    NOT NULL,
    [Login]                 VARCHAR (50)     NOT NULL,
    [Senha]                 VARCHAR (250)    NULL,
    [Email]                 VARCHAR (250)    NOT NULL,
    [IdPerfil]              UNIQUEIDENTIFIER NOT NULL,
    [IdArquivo]             UNIQUEIDENTIFIER NULL,
    [ResetarSenha]          BIT              CONSTRAINT [DF_Usuario_ResetarSenha] DEFAULT ((0)) NULL,
    [CodigoSenha]           VARCHAR (50)     NULL,
    [UltimoAcesso]          DATETIME         NULL,
    [Ativo]                 BIT              CONSTRAINT [DF_Usuario_Ativo] DEFAULT ((1)) NOT NULL,
    [DataNascimento]        DATE             NULL,
    [IdSetor]               UNIQUEIDENTIFIER NULL,
    [Telefone]              VARCHAR (20)     NULL,
    [Registro]              VARCHAR (50)     NULL,
    [Endereco]              VARCHAR (255)    NULL,
    [Bairro]                VARCHAR (50)     NULL,
    [Cidade]                VARCHAR (50)     NULL,
    [Escolaridade]          VARCHAR (250)    NULL,
    [EstadoCivil]           VARCHAR (250)    NULL,
    [IdTipoUsuario]         UNIQUEIDENTIFIER NULL,
    [ComoSerChamado]        VARCHAR (100)    NULL,
    [EmailAniversariante]   BIT              NULL,
    [Celular]               VARCHAR (20)     NULL,
    [NomeContatoEmergencia] VARCHAR (255)    NULL,
    [ContatoEmergencia]     VARCHAR (20)     NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Usuario_Setor] FOREIGN KEY ([IdSetor]) REFERENCES [dbo].[Setor] ([Id]),
    CONSTRAINT [FK_Usuario_TipoUsuario] FOREIGN KEY ([IdTipoUsuario]) REFERENCES [dbo].[TipoUsuario] ([Id])
);







