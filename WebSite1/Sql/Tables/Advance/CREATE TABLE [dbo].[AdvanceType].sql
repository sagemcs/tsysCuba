USE [PortalProveedoresProd061222]
GO

/****** Object:  Table [dbo].[AdvanceType]    Script Date: 20/1/2023 11:52:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AdvanceType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AdvanceType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--Insertar Datos por Defecto en tipo de anticipo
INSERT INTO [dbo].[AdvanceType] ([Id],[Name])  VALUES (1 ,'Viaje')
INSERT INTO [dbo].[AdvanceType] ([Id],[Name])  VALUES (1 ,'Compra Extraordinaria')