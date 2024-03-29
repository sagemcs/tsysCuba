USE [PortalProveedoresProd_Error]
GO
/****** Object:  StoredProcedure [dbo].[spRevPago]    Script Date: 10/02/2023 4:34:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER proc [dbo].[spRevPago]
@Opcion INT,
@VendKey INT,
@IDC varchar(5),
@Fol varchar(25)

AS
DECLARE @ERRORGEN INT
DECLARE @IDERROR INT		
DECLARE @ERROR_MESSAGE		NVARCHAR(2048)
DECLARE @ERROR_NUMBER		INT
DECLARE @ERROR_PROCEDURE	NVARCHAR(126)
DECLARE @DB_NAME			NVARCHAR(128)
DECLARE @ERROR_LINE			INT
DECLARE @ERROR_SEVERITY		INT
DECLARE @EMPRESA VARCHAR(225)
BEGIN TRY	
		
		IF(@Opcion = 4)
		BEGIN
			CREATE TABLE #Pagos(
			FechaPago varchar(30),
			Monto nvarchar(45),
			Factura varchar(255),
			NoCta nvarchar(45),
			Banco nvarchar(80),
			RFC_Banco nvarchar(15),
			Moneda nvarchar(5)
			)
			
			DECLARE @Folio INT
			DECLARE @Fecha Datetime
			DECLARE @Total Decimal(16,4)
			DECLARE @Cta Varchar(40)
			DECLARE @Banco Varchar(256)
			DECLARE @RFC Varchar(15)
			DECLARE @Moneda nvarchar(5)
			DECLARE @FKey INT
			DECLARE @PKey INT

			DECLARE ItemsPmt_Cursor cursor for	
			Select distinct 
			a.BatchKey AS Lote,
			a.TranDate As FechaPago,
			a.TranAmt As Total,
			e.CashAcctNo As NoCta,
			F.BankID As Banco,
			f.RoutingTransitNo As RFC_Banco,
			a.CurrID As Moneda

			From [mas500_u_app].dbo.tapVendPmt  a
			INNER JOIN [mas500_u_app].dbo.tapVendPmtAppl b ON a.VendPmtKey = b.ApplyFromPmtKey 
			INNER JOIN [mas500_u_app].dbo.tapVoucher c ON b.ApplyToVouchKey = c.VoucherKey
			INNER JOIN Invoice d ON c.TranNo = d.Folio and c.VendKey = d.VendorKey
			INNER JOIN [mas500_u_app].dbo.tcmCashAcct e ON a.CashAcctKey = e.CashAcctKey
			INNER JOIN [mas500_u_app].dbo.tcmBank f ON e.BankKey = f.BankKey
			INNER JOIN Notificaciones_Payment g on d.InvoiceKey = g.InvKey 
			Where a.VendKey =  @VendKey AND a.CompanyID = @IDC And d.Status = 7 AND g.Notificado = 0

			FOR  READ ONLY
			OPEN ItemsPmt_Cursor
			FETCH NEXT FROM ItemsPmt_Cursor INTO 
			@Folio,@Fecha,@Total,@Cta,@Banco,@RFC,@Moneda
	
			WHILE @@FETCH_STATUS = 0
			BEGIN
					
					Declare @Factura nvarchar(256)
					Declare @Folios nvarchar(256)
					SET @Folios = ''
					SET @Factura = ''
					DECLARE Facturas cursor for

					Select c.TranNo 
					from [mas500_u_app].dbo.tapVendPmt  a
					INNER JOIN [mas500_u_app].dbo.tapVendPmtAppl b ON a.VendPmtKey = b.ApplyFromPmtKey 
					INNER JOIN [mas500_u_app].dbo.tapVoucher c ON b.ApplyToVouchKey = c.VoucherKey
					Where a.BatchKey = @Folio

					FOR  READ ONLY
					OPEN Facturas
					FETCH NEXT FROM Facturas INTO @Factura
					WHILE @@FETCH_STATUS = 0
					BEGIN
					SET @Folios = @Folios + @Factura + ', '
					FETCH NEXT FROM Facturas INTO 
					@Factura
					END -- CURSOR

					CLOSE Facturas
					DEALLOCATE Facturas
					SET @Folios = SUBSTRING (@Folios, 1, Len(@Folios) - 2 )
					Declare @Fec varchar(30)
					SET @Fec = convert(varchar, @Fecha, 106) 
					Declare @Tota varchar(40)
					SET @Tota = CONVERT(varchar, convert(money, @Total), 1)

			Insert into #Pagos (FechaPago,Monto,Factura,NoCta,Banco,RFC_Banco,Moneda) Values (@Fec,@Tota,@Folios,@Cta,@Banco,@RFC,@MONEDA)

			FETCH NEXT FROM ItemsPmt_Cursor INTO 
			@Folio,@Fecha,@Total,@Cta,@Banco,@RFC,@Moneda
	
			END -- CURSOR

			CLOSE ItemsPmt_Cursor
			DEALLOCATE ItemsPmt_Cursor
			
			Select * From #Pagos
			Drop Table #Pagos

		END

		IF(@Opcion = 2)
		BEGIN 

			DECLARE @Folio1 INT
			DECLARE @Total1 Decimal(16,4)
			DECLARE @Vend1 Varchar(15)
			DECLARE @IDC1 varchar(5)
			DECLARE @PagosKey INT
			DECLARE @FacKey INT

			DECLARE ItemsPmt_Cursor cursor for	
			Select distinct 
			a.BatchKey AS Lote,
			a.TranAmt As Total,
			a.VendKey AS Vendor,
			a.CompanyID As Company,
			g.Pkey_Sage As KeyPago,
			g.InvKey as FKey

			From [mas500_u_app].dbo.tapVendPmt  a
			INNER JOIN [mas500_u_app].dbo.tapVendPmtAppl b ON a.VendPmtKey = b.ApplyFromPmtKey 
			INNER JOIN [mas500_u_app].dbo.tapVoucher c ON b.ApplyToVouchKey = c.VoucherKey
			INNER JOIN Invoice d ON c.TranNo = d.Folio and c.VendKey = d.VendorKey
			INNER JOIN Notificaciones_Payment g on d.InvoiceKey = g.InvKey 
			Where a.VendKey =  @VendKey AND a.CompanyID = @IDC And d.Status = 7 AND g.Notificado = 0

			FOR  READ ONLY
			OPEN ItemsPmt_Cursor
			FETCH NEXT FROM ItemsPmt_Cursor INTO 
			@Folio1,@Total1,@Vend1,@IDC1,@PagosKey,@FacKey
	
			WHILE @@FETCH_STATUS = 0
			BEGIN
					
					Declare @Factura1 nvarchar(256)
					SET @Factura1 = ''
					DECLARE Facturas cursor for

					Select c.TranNo 
					from [mas500_u_app].dbo.tapVendPmt  a
					INNER JOIN [mas500_u_app].dbo.tapVendPmtAppl b ON a.VendPmtKey = b.ApplyFromPmtKey 
					INNER JOIN [mas500_u_app].dbo.tapVoucher c ON b.ApplyToVouchKey = c.VoucherKey
					Where a.BatchKey = @Folio1

					FOR  READ ONLY
					OPEN Facturas
					FETCH NEXT FROM Facturas INTO @Factura1
					WHILE @@FETCH_STATUS = 0
					BEGIN

					BEGIN TRY
						  BEGIN TRAN

						Update Invoice Set Status = 7 Where Folio = @Factura1 And VendorKey = @Vend1 And CompanyID = @IDC1
						IF @@ERROR <>0 GOTO Error_Capturados

						Update Notificaciones_Payment Set Notificado = 1 Where InvKey = @FacKey AND Pkey_Sage = @PagosKey And VendKey = @Vend1 And Company = @IDC1
						IF @@ERROR <>0 GOTO Error_Capturados

					COMMIT TRAN

					Error_Capturados:
					IF @@ERROR <> 0
					BEGIN
					ROLLBACK
					END

					END TRY
					BEGIN CATCH
						ROLLBACK TRAN		
					END CATCH

					
					FETCH NEXT FROM Facturas INTO 
					@Factura1
					END -- CURSOR

					CLOSE Facturas
					DEALLOCATE Facturas


			FETCH NEXT FROM ItemsPmt_Cursor INTO 
			@Folio1,@Total1,@Vend1,@IDC1,@PagosKey,@FacKey
	
			END -- CURSOR

			CLOSE ItemsPmt_Cursor
			DEALLOCATE ItemsPmt_Cursor

		END

		IF(@Opcion = 6)
		BEGIN
			Declare @VendorKey INT
			Declare @InvcKey INT
			Declare @PagoKey INT
			Declare @Hoy Datetime
			Set @Hoy = getdate();

			DECLARE ItemsPmt_Cursor cursor for	

			Select distinct  
			b.VendKey AS Vendor,
			c.InvoiceKey As InvoiceKey,
			d.VendPmtKey As KeyPago

			from [mas500_u_app].dbo.tapVendPmtAppl a 
			inner join [mas500_u_app].dbo.tapVendPmt d on d.VendPmtKey = a.ApplyFromPmtKey 
			inner join [mas500_u_app].dbo.tapVoucher b on a.ApplyToVouchKey = b.VoucherKey 
			inner join Invoice c on b.TranNo = c.Folio 
			AND b.VendKey = c.VendorKey 
			And c.CompanyID = c.CompanyID
			where c.Status = 6 And c.companyId = @IDC

			FOR  READ ONLY
			OPEN ItemsPmt_Cursor
			FETCH NEXT FROM ItemsPmt_Cursor INTO 
			@VendorKey,@InvcKey,@PagoKey
	
			WHILE @@FETCH_STATUS = 0
			BEGIN

				BEGIN TRY
					BEGIN TRAN

					Update Invoice Set Status = 7 Where InvoiceKey = @InvcKey And VendorKey = @VendorKey And CompanyID = @IDC
					IF @@ERROR <>0 GOTO Error_Capturado

					Insert into [dbo].[Notificaciones_Payment] (VendKey,InvKey,Notificado,Fecha_Update_Payment,Pkey_Sage,Company) Values (@VendorKey,@InvcKey,0,@Hoy,@PagoKey,@IDC)
					IF @@ERROR <>0 GOTO Error_Capturado

					COMMIT TRAN

					Error_Capturado:
					IF @@ERROR <> 0
					BEGIN
					ROLLBACK
					END

				END TRY
				BEGIN CATCH
					ROLLBACK TRAN			
				END CATCH


			FETCH NEXT FROM ItemsPmt_Cursor INTO 
			@VendorKey,@InvcKey,@PagoKey
	
			END -- CURSOR

			CLOSE ItemsPmt_Cursor
			DEALLOCATE ItemsPmt_Cursor

		END

		IF(@Opcion = 8)
		BEGIN
			CREATE TABLE #Pagos2(
			FechaPago varchar(30),
			Monto nvarchar(45),
			Factura varchar(255),
			NoCta nvarchar(45),
			Banco nvarchar(80),
			RFC_Banco nvarchar(15),
			Moneda nvarchar(5)
			)
			
			DECLARE @Folio2 INT
			DECLARE @Fecha2 Datetime
			DECLARE @Total2 Decimal(16,4)
			DECLARE @Cta2 Varchar(40)
			DECLARE @Banco2 Varchar(256)
			DECLARE @RFC2 Varchar(15)
			DECLARE @Moneda2 nvarchar(5)
			DECLARE @FKey2 INT
			DECLARE @PKey2 INT

			DECLARE ItemsPmt_Cursor cursor for	
			Select distinct 
			a.BatchKey AS Lote,
			a.TranDate As FechaPago,
			a.TranAmt As Total,
			e.CashAcctNo As NoCta,
			F.BankID As Banco,
			f.RoutingTransitNo As RFC_Banco,
			a.CurrID As Moneda

			From [mas500_u_app].dbo.tapVendPmt  a
			INNER JOIN [mas500_u_app].dbo.tapVendPmtAppl b ON a.VendPmtKey = b.ApplyFromPmtKey 
			INNER JOIN [mas500_u_app].dbo.tapVoucher c ON b.ApplyToVouchKey = c.VoucherKey
			INNER JOIN Invoice d ON c.TranNo = d.Folio and c.VendKey = d.VendorKey
			INNER JOIN [mas500_u_app].dbo.tcmCashAcct e ON a.CashAcctKey = e.CashAcctKey
			INNER JOIN [mas500_u_app].dbo.tcmBank f ON e.BankKey = f.BankKey
			INNER JOIN Notificaciones_Payment g on d.InvoiceKey = g.InvKey
			INNER JOIN vendors l on l.vendorkey = d.vendorkey 
			Where l.superior =  @VendKey AND a.CompanyID = @IDC And d.Status = 7 AND g.Notificado = 0

			FOR  READ ONLY
			OPEN ItemsPmt_Cursor
			FETCH NEXT FROM ItemsPmt_Cursor INTO 
			@Folio2,@Fecha2,@Total2,@Cta2,@Banco2,@RFC2,@Moneda2
	
			WHILE @@FETCH_STATUS = 0
			BEGIN
					
					Declare @Factura2 nvarchar(256)
					Declare @Folios2 nvarchar(256)
					SET @Folios2 = ''
					SET @Factura2 = ''
					DECLARE Facturas cursor for

					Select c.TranNo 
					from [mas500_u_app].dbo.tapVendPmt  a
					INNER JOIN [mas500_u_app].dbo.tapVendPmtAppl b ON a.VendPmtKey = b.ApplyFromPmtKey 
					INNER JOIN [mas500_u_app].dbo.tapVoucher c ON b.ApplyToVouchKey = c.VoucherKey
					Where a.BatchKey = @Folio2

					FOR  READ ONLY
					OPEN Facturas
					FETCH NEXT FROM Facturas INTO @Factura2
					WHILE @@FETCH_STATUS = 0
					BEGIN
					SET @Folios2 = @Folios2 + @Factura2 + ', '
					FETCH NEXT FROM Facturas INTO 
					@Factura2
					END -- CURSOR

					CLOSE Facturas
					DEALLOCATE Facturas
					SET @Folios2 = SUBSTRING (@Folios2, 1, Len(@Folios2) - 2 )
					Declare @Fec2 varchar(30)
					SET @Fec2 = convert(varchar, @Fecha2, 106) 
					Declare @Tota2 varchar(40)
					SET @Tota2 = CONVERT(varchar, convert(money, @Total2), 1)

			Insert into #Pagos2 (FechaPago,Monto,Factura,NoCta,Banco,RFC_Banco,Moneda) Values (@Fec2,@Tota2,@Folios2,@Cta2,@Banco2,@RFC2,@MONEDA2)

			FETCH NEXT FROM ItemsPmt_Cursor INTO 
			@Folio2,@Fecha2,@Total2,@Cta2,@Banco2,@RFC2,@Moneda2
	
			END -- CURSOR

			CLOSE ItemsPmt_Cursor
			DEALLOCATE ItemsPmt_Cursor
			
			Select * From #Pagos2
			Drop Table #Pagos2

		END

		IF(@Opcion = 9)
		BEGIN 

			DECLARE @Folio3 INT
			DECLARE @Total3 Decimal(16,4)
			DECLARE @Vend3 Varchar(15)
			DECLARE @IDC3 varchar(5)
			DECLARE @PagosKey3 INT
			DECLARE @FacKey3 INT

			DECLARE ItemsPmt_Cursor cursor for	
			Select distinct 
			a.BatchKey AS Lote,
			a.TranAmt As Total,
			a.VendKey AS Vendor,
			a.CompanyID As Company,
			g.Pkey_Sage As KeyPago,
			g.InvKey as FKey

			From [mas500_u_app].dbo.tapVendPmt  a
			INNER JOIN [mas500_u_app].dbo.tapVendPmtAppl b ON a.VendPmtKey = b.ApplyFromPmtKey 
			INNER JOIN [mas500_u_app].dbo.tapVoucher c ON b.ApplyToVouchKey = c.VoucherKey
			INNER JOIN Invoice d ON c.TranNo = d.Folio and c.VendKey = d.VendorKey
			INNER JOIN Notificaciones_Payment g on d.InvoiceKey = g.InvKey
			INNER JOIN vendors l on l.vendorkey = d.vendorkey 
			Where l.superior =  @VendKey AND a.CompanyID = @IDC And d.Status = 7 AND g.Notificado = 0

			FOR  READ ONLY
			OPEN ItemsPmt_Cursor
			FETCH NEXT FROM ItemsPmt_Cursor INTO 
			@Folio3,@Total3,@Vend3,@IDC3,@PagosKey3,@FacKey3
	
			WHILE @@FETCH_STATUS = 0
			BEGIN
					
					Declare @Factura3 nvarchar(256)
					SET @Factura3 = ''
					DECLARE Facturas cursor for

					Select c.TranNo 
					from [mas500_u_app].dbo.tapVendPmt  a
					INNER JOIN [mas500_u_app].dbo.tapVendPmtAppl b ON a.VendPmtKey = b.ApplyFromPmtKey 
					INNER JOIN [mas500_u_app].dbo.tapVoucher c ON b.ApplyToVouchKey = c.VoucherKey
					Where a.BatchKey = @Folio3

					FOR  READ ONLY
					OPEN Facturas
					FETCH NEXT FROM Facturas INTO @Factura3
					WHILE @@FETCH_STATUS = 0
					BEGIN

					BEGIN TRY
						  BEGIN TRAN

						Update Invoice Set Status = 7 Where Folio = @Factura3 And VendorKey = @Vend3 And CompanyID = @IDC3
						IF @@ERROR <>0 GOTO Error_Capturados3

						Update Notificaciones_Payment Set Notificado = 1 Where InvKey = @FacKey3 AND Pkey_Sage = @PagosKey3 And VendKey = @Vend3 And Company = @IDC3
						IF @@ERROR <>0 GOTO Error_Capturados3

					COMMIT TRAN

					Error_Capturados3:
					IF @@ERROR <> 0
					BEGIN
					ROLLBACK
					END

					END TRY
					BEGIN CATCH
						ROLLBACK TRAN		
					END CATCH

					
					FETCH NEXT FROM Facturas INTO 
					@Factura3
					END -- CURSOR

					CLOSE Facturas
					DEALLOCATE Facturas


			FETCH NEXT FROM ItemsPmt_Cursor INTO 
			@Folio3,@Total3,@Vend3,@IDC3,@PagosKey3,@FacKey3
	
			END -- CURSOR

			CLOSE ItemsPmt_Cursor
			DEALLOCATE ItemsPmt_Cursor

		END
		
END TRY
BEGIN CATCH
		SET @ERRORGEN = 1			
		SET @ERROR_MESSAGE = ERROR_MESSAGE()
		SET @ERROR_NUMBER = ERROR_NUMBER()
		SET @ERROR_PROCEDURE = ERROR_PROCEDURE()
		SET @DB_NAME = DB_NAME()
		SET @ERROR_LINE = ERROR_LINE()
		SET @ERROR_SEVERITY = ERROR_SEVERITY()
		SET @EMPRESA = NULL
		Select @EMPRESA As Empresas
END CATCH



