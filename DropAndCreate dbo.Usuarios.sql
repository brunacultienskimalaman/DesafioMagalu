USE [MagaluDb]
GO

/****** Object: Table [dbo].[Usuarios] Script Date: 28/02/2025 13:21:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Usuarios];


GO
CREATE TABLE [dbo].[Usuarios] (
	ID INT IDENTITY NOT NULL ,
    [UserID] INT            NOT NULL,
    [Name]   NVARCHAR (100) NOT NULL
);



DROP TABLE [dbo].[Produtos];


GO
CREATE TABLE [dbo].[Produtos] (
	ID INT IDENTITY NOT NULL ,
    [ProductId] INT             NOT NULL,
    [Value]     DECIMAL (18, 2) NOT NULL,
    [PedidoId]  INT             NOT NULL
);




DROP TABLE [dbo].[Pedidos];


GO
CREATE TABLE [dbo].[Pedidos] (
	ID INT IDENTITY NOT NULL ,
    [OrderId]   INT             NOT NULL,
    [Total]     DECIMAL (18, 2) NOT NULL,
    [Date]      DATETIME2 (7)   NOT NULL,
    [UsuarioId] INT             NOT NULL
);







