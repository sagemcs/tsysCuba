USE [PortalProveedoresProd061222]
GO

/****** Object:  Table [dbo].[CorporateCard]    Script Date: 16/1/2023 10:38:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[CorporateCard]

CREATE TABLE [dbo].[CorporateCard](
	[CorporateCardId] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Currency] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[UpdateUserKey] [int] NOT NULL,
	[CompanyId] [varchar](3) NOT NULL,
	[Status] [int] NOT NULL,
	[PackageId] [int] NULL,
	[DeniedReason] [nvarchar](256) NULL,
	[ApprovalLevel] [int] NULL,
	[ExpenseReason] [nvarchar](256) NULL,
	[SageIntegration] [bit] NULL,
 CONSTRAINT [PK_CorporateCard] PRIMARY KEY CLUSTERED 
(
	[CorporateCardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


