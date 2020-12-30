CREATE TABLE [dbo].[Banner] (
    [id]         UNIQUEIDENTIFIER CONSTRAINT [DF_Banner_Id] DEFAULT (newid()) NOT NULL,
    [modulo]     VARCHAR (50)     NOT NULL,
    [componente] VARCHAR (50)     NULL,
    [posicao]    INT              CONSTRAINT [DF_Banner_posicao] DEFAULT ((0)) NOT NULL,
    [caminho]    VARCHAR (200)    NULL,
    [texto1]     VARCHAR (100)    NULL,
    [texto2]     VARCHAR (100)    NULL,
    [texto3]     VARCHAR (100)    NULL,
    [link]       VARCHAR (100)    NULL,
    [cor]        VARCHAR (20)     NULL,
    [ativo]      BIT              CONSTRAINT [DF_Banner_ativo] DEFAULT ((1)) NOT NULL,
    [caminho2]   VARCHAR (200)    NULL,
    CONSTRAINT [PK_Banner] PRIMARY KEY CLUSTERED ([id] ASC)
);





