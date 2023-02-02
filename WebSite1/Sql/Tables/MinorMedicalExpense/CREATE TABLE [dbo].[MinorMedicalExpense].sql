USE [PortalProveedoresProd061222]
GO

/****** Object:  Table [dbo].[MinorMedicalExpense]    Script Date: 16/1/2023 10:40:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[MinorMedicalExpense]


CREATE TABLE [dbo].[MinorMedicalExpense](
	[MinorMedicalExpenseId] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[FileBinaryXml] [varbinary](max) NULL,
	[FileBinaryPdf] [varbinary](max) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[UpdateUserKey] [int] NOT NULL,
	[CompanyId] [varchar](3) NOT NULL,
	[Status] [int] NOT NULL,
	[PackageId] [int] NULL,
	[DeniedReason] [nvarchar](256) NULL,
	[FileNameXml] [nvarchar](256) NULL,
	[FileNamePdf] [nvarchar](256) NULL,
	[ApprovalLevel] [int] NULL,
	[SageIntegration] [bit] NULL,
 CONSTRAINT [PK_MinorMedicalExpense] PRIMARY KEY CLUSTERED 
(
	[MinorMedicalExpenseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


