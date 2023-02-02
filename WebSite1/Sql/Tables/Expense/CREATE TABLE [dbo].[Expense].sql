USE [PortalProveedoresProd061222]
GO

/****** Object:  Table [dbo].[Expense]    Script Date: 16/1/2023 10:39:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[Expense]

CREATE TABLE [dbo].[Expense](
	[ExpenseId] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Currency] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[UpdateUserKey] [int] NOT NULL,
	[CompanyId] [varchar](3) NOT NULL,
	[Status] [int] NOT NULL,
	[PackageId] [int] NULL,
	[DeniedReason] [nvarchar](256) NULL,
	[AdvanceId] [int] NOT NULL,
	[ApprovalLevel] [int] NULL,
	[ExpenseReason] [nvarchar](256) NULL,
	[SageIntegration] [bit] NULL,
 CONSTRAINT [PK_Expense] PRIMARY KEY CLUSTERED 
(
	[ExpenseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


