USE [PortalProveedoresProd061222]
GO

/****** Object:  Table [dbo].[DefaultItemMME]    Script Date: 16/1/2023 10:39:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[DefaultItemMME]

CREATE TABLE [dbo].[DefaultItemMME](
	[ItemKey] [int] NOT NULL,
	[CompanyID] [char](3) NOT NULL,
	[DateUpdated] [date] NOT NULL
) ON [PRIMARY]
GO


