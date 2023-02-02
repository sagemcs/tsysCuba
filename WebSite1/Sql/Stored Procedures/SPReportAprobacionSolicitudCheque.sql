USE [PortalProveedoresProd_Extra]
GO

/****** Object:  StoredProcedure [dbo].[SpReportAprobacionSolCheque]    Script Date: 02/02/2023 5:37:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Manuel Barreiro 
-- Create date: 4-12-2022
-- Description:	Reporte de aprobacion de solicitud de cheque
-- =============================================
ALTER PROCEDURE [dbo].[SpReportAprobacionSolCheque]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select InvoiceReceipt.InvcRcptKey,Vendors.VendorID,Vendors.VendName,InvoiceReceipt.folio as Solicitud,invoice.folio as Factura,
	ISNULL((select username from users where approInvcReceipt.Nivel1 = userkey) , '') as 'Cuentas por Pagar',
	ISNULL((select username from users where approInvcReceipt.Nivel2 = userkey) , '') as 'Dirección Tesorería',
	ISNULL((select username from users where approInvcReceipt.Nivel3 = userkey) , '') as 'Dirección Finanzas'      
	FROM InvoiceReceipt  
	inner JOIN approInvcReceipt on InvoiceReceipt.InvcRcptKey = approInvcReceipt.InvcRcptKey 
	inner JOIN invcRcptDetails  on InvoiceReceipt.InvcRcptKey = invcRcptDetails.InvcRcptKey 
	inner JOIN invoice on invcRcptDetails.InvoiceKey = invoice.InvoiceKey 
	inner JOIN Vendors on InvoiceReceipt.VendorKey = Vendors.VendorKey 
END
GO

