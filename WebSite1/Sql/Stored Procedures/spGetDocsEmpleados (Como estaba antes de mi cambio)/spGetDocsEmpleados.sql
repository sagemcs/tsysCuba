USE [PortalProveedoresProd_11012023]
GO
/****** Object:  StoredProcedure [dbo].[spGetDocsEmpleados]    Script Date: 10/02/2023 6:41:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER proc [dbo].[spGetDocsEmpleados]
@Opcion INT,
@UserKey INT,
@Prov nvarchar(250)
AS
BEGIN
	Declare @CompanyID nvarchar(5)
	SET @CompanyID  = (Select top 1 CompanyID from vendors where superior = @UserKey)
BEGIN TRY
BEGIN
	
	IF(@Opcion = 1)
	BEGIN
		SELECT d.Moneda,d.InvcRcptKey,d.Folio,f.VendName,c.VendDBA as RFC,t1.CreateDate,t1.PaymentDate,t1.Total,d.InvoiceKey, e.DueDayOrMonth AS condiciones, e.[Description] AS condiciones_desc
		FROM InvoiceReceipt t1 
		LEFT join mas500_u_app.dbo.TapVendor c on t1.vendorKey = c.VendKey ---AQUI CAMBIAR A PROD
		LEFT JOIN [mas500_u_app].dbo.tciPaymentTerms e ON c.PmtTermsKey = e.PmtTermsKey
		LEFT JOIN Vendors f on c.VendKey = f.VendorKey
		LEFT join InvcRcptDetails d on d.InvcRcptKey = t1.InvcRcptKey
		WHERE NOT EXISTS (SELECT NULL FROM ChkReqDetail t2  WHERE t2.InvcRcptKey = t1.InvcRcptKey) 
		AND f.Superior = @UserKey AND t1.CompanyID = @CompanyID
	END

	IF(@Opcion = 2)
	BEGIN
		Select CONVERT(VARCHAR(10), AprovDate, 103) As Fecha,c.Vendname as VendID,RFCEmisor as RFC,NodeOc as OC,b.TranNo as Folio,Moneda,b.TranAmt as Total, b.Balance as Saldo
		From invoice a inner join vendors c on a.VendorKey = c.VendorKey 
		inner join [mas500_u_app].dbo.tapVoucher b on a.Folio =b.TranNo AND a.VendorKey = b.VendKey
		Where a.Status >= 6 AND a.Status < 8
		AND c.Superior = @UserKey AND b.CompanyID = @CompanyID and c.Vendname = @Prov
	END

	IF(@Opcion = 3)
	BEGIN
		IF(@Prov = '0')
		BEGIN
			Select CONVERT(VARCHAR(10), AprovDate, 103) As Fecha,c.Vendname as VendID,RFCEmisor as RFC,NodeOc as OC,b.TranNo as Folio,Moneda,b.TranAmt as Total, b.Balance as Saldo
			From invoice a inner join vendors c on a.VendorKey = c.VendorKey 
			inner join [mas500_u_app].dbo.tapVoucher b on a.Folio =b.TranNo AND a.VendorKey = b.VendKey
			Where a.Status >= 6 AND a.Status < 8 
			and a.Folio + '-' + a.NodeOc not in (select UUID from Payment )
		END
		ELSE
		BEGIN
			Select CONVERT(VARCHAR(10), AprovDate, 103) As Fecha,c.Vendname as VendID,RFCEmisor as RFC,NodeOc as OC,b.TranNo as Folio,Moneda,b.TranAmt as Total, b.Balance as Saldo
			From invoice a inner join vendors c on a.VendorKey = c.VendorKey 
			inner join [mas500_u_app].dbo.tapVoucher b on a.Folio =b.TranNo AND a.VendorKey = b.VendKey
			Where a.Status >= 6 AND a.Status < 8 AND c.Vendname = @Prov
			and a.Folio + '-' + a.NodeOc not in (select UUID from Payment ) 
			--
			
		END
		--AND c.Superior = @UserKey AND b.CompanyID = @CompanyID and c.Vendname = @Prov
	END

END

END TRY 

BEGIN CATCH 
	INSERT INTO InvcLogValidacion(Vendkey,InvoiceKey,InvoiceLineKey,FechaError,ErrorValidacion,Proceso,eliminado,CompanyID) 
	VALUES (0,0,0,GETDATE(),ERROR_MESSAGE ( ),'spGetDocsEmpleados',0,@CompanyID) 
	RETURN END 
CATCH END
