USE [PortalProveedoresProd061222]
GO

/****** Object:  Table [dbo].[Advance]    Script Date: 16/1/2023 10:37:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[Advance]

CREATE TABLE [dbo].[Advance](
	[AdvanceId] [int] IDENTITY(1,1) NOT NULL,
	[AdvanceType] [int] NOT NULL,
	[Folio] [nvarchar](50) NOT NULL,
	[Currency] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[DepartureDate] [date] NULL,
	[ArrivalDate] [date] NULL,
	[CheckDate] [date] NOT NULL,
	[ImmediateBoss] [nvarchar](500) NOT NULL,
	[UpdateUserKey] [int] NOT NULL,
	[CompanyId] [nvarchar](3) NOT NULL,
	[Status] [int] NOT NULL,
	[PackageId] [int] NULL,
	[DeniedReason] [varchar](256) NULL,
	[CreateDate] [date] NOT NULL,
	[UpdateDate] [date] NOT NULL,
	[ApprovalLevel] [int] NULL,
	[ExpenseReason] [varchar](256) NULL,
	[SageIntegration] [bit] NULL
) ON [PRIMARY]
GO


