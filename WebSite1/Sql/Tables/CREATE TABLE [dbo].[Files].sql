USE [PortalProveedoresProd061222]
GO

/****** Object:  Table [dbo].[Files]    Script Date: 16/1/2023 10:39:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[Files]

CREATE TABLE [dbo].[Files](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExpenseType] [int] NOT NULL,
	[ExpenseId] [int] NOT NULL,
	[ExpenseDetailId] [int] NOT NULL,
	[FileName] [nvarchar](max) NOT NULL,
	[FileType] [int] NOT NULL,
	[FileBinary] [varbinary](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


