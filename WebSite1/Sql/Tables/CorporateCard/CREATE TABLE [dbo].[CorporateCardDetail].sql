USE [PortalProveedoresProd061222]
GO

/****** Object:  Table [dbo].[CorporateCardDetail]    Script Date: 16/1/2023 10:38:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE IF EXISTS [dbo].[CorporateCardDetail]


CREATE TABLE [dbo].[CorporateCardDetail](
	[DetailId] [int] IDENTITY(1,1) NOT NULL,
	[CorporateCardId] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[ItemKey] [int] NOT NULL,
	[Qty] [decimal](18, 2) NOT NULL,
	[UnitCost] [decimal](18, 2) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[STaxCodeKey] [int] NULL,
	[TaxAmount] [decimal](18, 2) NULL,
	[CreateDate] [date] NOT NULL,
	[UpdateDate] [date] NOT NULL,
	[CreateUser] [int] NOT NULL,
	[CompanyId] [varchar](3) NOT NULL
) ON [PRIMARY]
GO


