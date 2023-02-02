USE [PortalProveedoresProd_Extra]
GO

/****** Object:  StoredProcedure [dbo].[spGetDocsEmpleados]    Script Date: 27/01/2023 5:31:02 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[spGetDocsEmpleados]
@Opcion INT,
@UserKey INT,
@Prov nvarchar(250),
@Fecha INT,
@FechaDesde nvarchar(250),
@FechaHasta nvarchar(250),
@Status nvarchar,
@Folio nvarchar(250)
AS
BEGIN
	Declare @CompanyID nvarchar(5)
	DECLARE @FILTRO Varchar(max)
	Declare @SQLString varchar(max)
	SET @CompanyID  = (Select top 1 CompanyID from vendors where superior = @UserKey)
BEGIN TRY
BEGIN
	
	IF(@Opcion = 1)
	BEGIN
		SELECT d.Moneda,d.InvcRcptKey,d.Folio,f.VendName,c.VendDBA as RFC,t1.CreateDate,t1.PaymentDate,t1.Total,d.InvoiceKey, e.DueDayOrMonth AS condiciones, e.[Description] AS condiciones_desc
		FROM InvoiceReceipt t1 
		LEFT join sage500_Portal081122.dbo.TapVendor c on t1.vendorKey = c.VendKey ---AQUI CAMBIAR A PROD
		LEFT JOIN [sage500_Portal081122].dbo.tciPaymentTerms e ON c.PmtTermsKey = e.PmtTermsKey
		LEFT JOIN Vendors f on c.VendKey = f.VendorKey
		LEFT join InvcRcptDetails d on d.InvcRcptKey = t1.InvcRcptKey
		WHERE NOT EXISTS (SELECT NULL FROM ChkReqDetail t2  WHERE t2.InvcRcptKey = t1.InvcRcptKey) 
		AND f.Superior = @UserKey AND t1.CompanyID = @CompanyID
	END

	IF(@Opcion = 2)
	BEGIN
		Select CONVERT(VARCHAR(10), AprovDate, 103) As Fecha,c.Vendname as VendID,RFCEmisor as RFC,NodeOc as OC,b.TranNo as Folio,Moneda,b.TranAmt as Total, b.Balance as Saldo
		From invoice a inner join vendors c on a.VendorKey = c.VendorKey 
		inner join [sage500_Portal081122].dbo.tapVoucher b on a.Folio =b.TranNo AND a.VendorKey = b.VendKey
		Where a.Status >= 6 AND a.Status < 8
		AND c.Superior = @UserKey AND b.CompanyID = @CompanyID and c.Vendname = @Prov
	END

	IF(@Opcion = 3)
	BEGIN
		IF(@Prov = '0')
		BEGIN
			--Select CONVERT(VARCHAR(10), AprovDate, 103) As Fecha,c.Vendname as VendID,RFCEmisor as RFC,NodeOc as OC,b.TranNo as Folio,Moneda,b.TranAmt as Total, b.Balance as Saldo
			--From invoice a inner join vendors c on a.VendorKey = c.VendorKey 
			--inner join [sage500_Portal081122].dbo.tapVoucher b on a.Folio =b.TranNo AND a.VendorKey = b.VendKey
			--Where a.Status >= 6 AND a.Status < 8 
			--and a.Folio + '-' + a.NodeOc not in (select UUID from Payment )

			SET @FILTRO = ' and a.Status = ' + @Status + ''
			if(@Folio <> '')
				SET @FILTRO = @FILTRO + ' AND a.Folio = ''' + @Folio + ''''

			if(@Fecha = 0)
			Begin
				if(@FechaDesde <> '')
					SET @FILTRO = @FILTRO + ' AND CONVERT(DATE,AprovDate)  >= ''' + @FechaDesde + ''''
				if(@FechaHasta <> '')
					SET @FILTRO = @FILTRO + ' AND CONVERT(DATE,AprovDate) <= ''' + @FechaHasta + ''''
			end
				--SET @FILTRO = @FILTRO + ' AND CONVERT(DATE,AprovDate)  >= ''' + @FechaDesde + '''' + ' AND CONVERT(DATE,AprovDate) <= ''' + @FechaHasta + ''''
			if(@Fecha = 1)
			Begin
				if(@FechaDesde <> '')
					SET @FILTRO = @FILTRO + ' AND CONVERT(DATE,a.FechaTransaccion)  >= ''' + @FechaDesde + ''''
				if(@FechaHasta <> '')
					SET @FILTRO = @FILTRO + ' AND CONVERT(DATE,a.FechaTransaccion) <= ''' + @FechaHasta + ''''
			end
				--SET @FILTRO = @FILTRO + ' AND CONVERT(DATE,a.FechaTransaccion)  >= ''' + @FechaDesde + '''' + ' AND CONVERT(DATE,a.FechaTransaccion) <= ''' + @FechaHasta + ''''

			--select a.Status,CONVERT(VARCHAR(10), AprovDate, 103) As Fecha,a.Serie,a.Folio,c.Vendname as VendID,RFCEmisor as RFC,NodeOc as OC,b.TranNo as Folio,a.Moneda,
			--b.TranAmt as Total,b.Balance as Saldo,case when a.FechaTransaccion is not null then convert(varchar, a.FechaTransaccion, 3) else '' end as FechaFactura,
			--case when a.UpdateDate is not null then convert(varchar, a.UpdateDate, 3) else '' end as FechaRecep,case when a.AprovDate is not null then convert(varchar, a.AprovDate, 3) else '' end FechaAprob,
			--a.Subtotal,a.ImpuestoImporteTrs as Impuestos,a.Total,ir.Folio as FolioContrarecibo,cr.Serie as FolioSolChqk,convert(varchar, ir.PaymentDate, 3) as FechaProgPago,
			--F.BankID as BancoPago,e.CashAcctNo as Cuenta,(select convert(VARCHAR(10),Fecha_Update_Payment,103) From Notificaciones_Payment Where InvKey = a.InvoiceKey) as FechaNot,
			--(select p.Folio From PaymentAppl pa inner join Payment p on pa.PaymentKey = p.PaymentKey Where pa.ApplInvoiceKey = a.InvoiceKey) as FolioComplPago,
			--(select convert(varchar, p.CreateDate, 3) From PaymentAppl pa inner join Payment p on pa.PaymentKey = p.PaymentKey Where pa.ApplInvoiceKey = a.InvoiceKey) as FechaRecepComplPago,
			--(select convert(varchar, p.AprovDate, 3) From PaymentAppl pa inner join Payment p on pa.PaymentKey = p.PaymentKey Where pa.ApplInvoiceKey = a.InvoiceKey) as FechaAprobComplPago
			--From invoice a inner join vendors c on a.VendorKey = c.VendorKey
			--inner join [sage500_Portal081122].dbo.tapVoucher b on a.Folio = b.TranNo AND a.VendorKey = b.VendKey
			--left join InvcRcptDetails id on a.InvoiceKey = id.InvoiceKey
			--left join InvoiceReceipt ir on id.InvcRcptKey = ir.InvcRcptKey
			--left join ChkReqDetail crd on ir.InvcRcptKey = crd.InvcRcptKey
			--left join CheckRequest cr on crd.ChkReqKey = cr.ChkReqKey
			--left JOIN [sage500_Portal081122].dbo.tapVendPmtAppl tvpa on tvpa.ApplyToVouchKey = b.VoucherKey
			--left join [sage500_Portal081122].dbo.tapVendPmt tvp on tvp.VendPmtKey = tvpa.ApplyFromPmtKey
			--left JOIN [sage500_Portal081122].dbo.tcmCashAcct e ON tvp.CashAcctKey = e.CashAcctKey
			--left JOIN [sage500_Portal081122].dbo.tcmBank f ON e.BankKey = f.BankKey
			--WHERE a.Status >= 6 AND a.Status < 8 and a.Folio  + '-'  + a.NodeOc not in (select UUID from Payment)


			set @SQLString = N'select CONVERT(VARCHAR(10), AprovDate, 103) As Fecha,a.Serie,a.Folio,c.Vendname as VendID,RFCEmisor as RFC,NodeOc as OC,b.TranNo as Folio,a.Moneda,'
			set @SQLString = @SQLString + 'b.TranAmt as Total,b.Balance as Saldo,case when a.FechaTransaccion is not null then convert(varchar, a.FechaTransaccion, 3) else '''' end as FechaFactura,'
			set @SQLString = @SQLString + 'case when a.UpdateDate is not null then convert(varchar, a.UpdateDate, 3) else '''' end as FechaRecep,case when a.AprovDate is not null then convert(varchar, a.AprovDate, 3) else '''' end FechaAprob,'
			set @SQLString = @SQLString + 'a.Subtotal,a.ImpuestoImporteTrs as Impuestos,a.Total,ir.Folio as FolioContrarecibo,cr.Serie as FolioSolChqk,convert(varchar, ir.PaymentDate, 3) as FechaProgPago,'
			set @SQLString = @SQLString + 'F.BankID as BancoPago,e.CashAcctNo as Cuenta,(select convert(VARCHAR(10),Fecha_Update_Payment,103) From Notificaciones_Payment Where InvKey = a.InvoiceKey) as FechaNot,'
			set @SQLString = @SQLString + '(select p.Folio From PaymentAppl pa inner join Payment p on pa.PaymentKey = p.PaymentKey Where pa.ApplInvoiceKey = a.InvoiceKey) as FolioComplPago,'
			set @SQLString = @SQLString + '(select convert(varchar, p.CreateDate, 3) From PaymentAppl pa inner join Payment p on pa.PaymentKey = p.PaymentKey Where pa.ApplInvoiceKey = a.InvoiceKey) as FechaRecepComplPago,'
			set @SQLString = @SQLString + '(select convert(varchar, p.AprovDate, 3) From PaymentAppl pa inner join Payment p on pa.PaymentKey = p.PaymentKey Where pa.ApplInvoiceKey = a.InvoiceKey) as FechaAprobComplPago'
			set @SQLString = @SQLString + ' From invoice a inner join vendors c on a.VendorKey = c.VendorKey'
			set @SQLString = @SQLString + ' inner join [sage500_Portal081122].dbo.tapVoucher b on a.Folio = b.TranNo AND a.VendorKey = b.VendKey'
			set @SQLString = @SQLString + ' left join InvcRcptDetails id on a.InvoiceKey = id.InvoiceKey'
			set @SQLString = @SQLString + ' left join InvoiceReceipt ir on id.InvcRcptKey = ir.InvcRcptKey'
			set @SQLString = @SQLString + ' left join ChkReqDetail crd on ir.InvcRcptKey = crd.InvcRcptKey'
			set @SQLString = @SQLString + ' left join CheckRequest cr on crd.ChkReqKey = cr.ChkReqKey'
			set @SQLString = @SQLString + ' left JOIN [sage500_Portal081122].dbo.tapVendPmtAppl tvpa on tvpa.ApplyToVouchKey = b.VoucherKey'
			set @SQLString = @SQLString + ' left join [sage500_Portal081122].dbo.tapVendPmt tvp on tvp.VendPmtKey = tvpa.ApplyFromPmtKey'
			set @SQLString = @SQLString + ' left JOIN [sage500_Portal081122].dbo.tcmCashAcct e ON tvp.CashAcctKey = e.CashAcctKey'
			set @SQLString = @SQLString + ' left JOIN [sage500_Portal081122].dbo.tcmBank f ON e.BankKey = f.BankKey'
			set @SQLString = @SQLString + ' WHERE a.Status >= 6 AND a.Status < 8'
			set @SQLString = @SQLString +  ' and (a.Folio + ' + '''-''' + ' + a.NodeOc not in (select UUID from Payment))'

			SET @SQLString = @SQLString + @FILTRO ;
			EXEC(@SQLString)

		END
		ELSE
		BEGIN
			Select CONVERT(VARCHAR(10), AprovDate, 103) As Fecha,c.Vendname as VendID,RFCEmisor as RFC,NodeOc as OC,b.TranNo as Folio,Moneda,b.TranAmt as Total, b.Balance as Saldo
			From invoice a inner join vendors c on a.VendorKey = c.VendorKey 
			inner join [sage500_Portal081122].dbo.tapVoucher b on a.Folio =b.TranNo AND a.VendorKey = b.VendKey
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
GO

