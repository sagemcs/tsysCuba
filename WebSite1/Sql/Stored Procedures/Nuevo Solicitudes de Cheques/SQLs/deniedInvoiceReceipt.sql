USE [PortalProveedoresProd_Extra]
GO

/****** Object:  Table [dbo].[deniedInvoiceReceipt]    Script Date: 20/01/2023 8:08:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[deniedInvoiceReceipt](
	[InvcRcptKey] [int] NOT NULL,
	[FechaRechazo] [datetime] NOT NULL
) ON [PRIMARY]
GO

