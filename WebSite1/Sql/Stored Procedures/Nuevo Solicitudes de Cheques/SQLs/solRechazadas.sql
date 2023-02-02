USE [PortalProveedoresProd_Extra]
GO

/****** Object:  StoredProcedure [dbo].[SpReportRechazoSolCheque]    Script Date: 20/01/2023 8:14:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Manuel Barreiro 
-- Create date: 20-01-2022
-- Description:	Reporte de rechazo de solicitud de cheque
-- =============================================
CREATE PROCEDURE [dbo].[SpReportRechazoSolCheque]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select InvoiceReceipt.InvcRcptKey,Vendors.VendorID,Vendors.VendName,InvoiceReceipt.folio as Solicitud,invoice.folio as Factura,
	ISNULL((select username from users where InvoiceReceipt.Rechazador = userkey) , '') as 'Rechazo',
	InvoiceReceipt.Comment as 'Motivo',
	ISNULL(convert(varchar, deniedInvoiceReceipt.FechaRechazo, 3) , '') as 'Fecha'
	FROM InvoiceReceipt  
	inner JOIN deniedInvoiceReceipt on InvoiceReceipt.InvcRcptKey = deniedInvoiceReceipt.InvcRcptKey 
	inner JOIN invcRcptDetails  on InvoiceReceipt.InvcRcptKey = invcRcptDetails.InvcRcptKey 
	inner JOIN invoice on invcRcptDetails.InvoiceKey = invoice.InvoiceKey 
	inner JOIN Vendors on InvoiceReceipt.VendorKey = Vendors.VendorKey 
END

GO

