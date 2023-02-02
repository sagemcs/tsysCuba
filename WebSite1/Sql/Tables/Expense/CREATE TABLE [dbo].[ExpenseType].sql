USE [PortalProveedoresProd061222]
GO

/****** Object:  Table [dbo].[ExpenseType]    Script Date: 20/1/2023 15:52:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ExpenseType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ExpenseType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


INSERT INTO [dbo].[ExpenseType] ([Id],[Name])VALUES (1,'Transporte Aéreo')
  INSERT INTO [dbo].[ExpenseType] ([Id],[Name])VALUES (2,'Transporte Terrestre')
  INSERT INTO [dbo].[ExpenseType] ([Id],[Name])VALUES (3,'Casetas')
  INSERT INTO [dbo].[ExpenseType] ([Id],[Name])VALUES (4,'Gasolina')
  INSERT INTO [dbo].[ExpenseType] ([Id],[Name])VALUES (5,'Estacionamiento')
  INSERT INTO [dbo].[ExpenseType] ([Id],[Name])VALUES (6,'Alimentos')
  INSERT INTO [dbo].[ExpenseType] ([Id],[Name])VALUES (7,'Hospedaje')
  INSERT INTO [dbo].[ExpenseType] ([Id],[Name])VALUES (8,'Gastos extraordinarios')


