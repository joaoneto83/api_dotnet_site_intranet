CREATE TABLE [dbo].[PedidoProduto] (
    [IdPedido]          UNIQUEIDENTIFIER NOT NULL,
    [IdProduto]         UNIQUEIDENTIFIER NOT NULL,
    [QtdProduto]        INT              NOT NULL,
    [ValorTotalProduto] DECIMAL (10, 2)  NOT NULL,
    CONSTRAINT [FK_PedidoProduto_Pedido] FOREIGN KEY ([IdPedido]) REFERENCES [dbo].[Pedido] ([Id]),
    CONSTRAINT [FK_PedidoProduto_Produto] FOREIGN KEY ([IdProduto]) REFERENCES [dbo].[Produto] ([Id])
);

