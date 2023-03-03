USE [PortalProveedoresProd_Error]
GO
/****** Object:  StoredProcedure [dbo].[spUpdateInvoice]    Script Date: 10/02/2023 4:34:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER Procedure [dbo].[spUpdateInvoice]
@Company VARCHAR(5)
AS
BEGIN
Declare @Hoy as Datetime
DECLARE @ERRORGEN INT
DECLARE @IDERROR INT		
DECLARE @ERROR_MESSAGE		NVARCHAR(2048)
DECLARE @ERROR_NUMBER		INT
DECLARE @ERROR_PROCEDURE	NVARCHAR(126)
DECLARE @DB_NAME			NVARCHAR(128)
DECLARE @ERROR_LINE			INT
DECLARE @ERROR_SEVERITY		INT	
Set @Hoy = getdate();
	
-- Registra en Tabla Users
		BEGIN TRY
		BEGIN TRAN

		--Declare @invcKey INT
		--DECLARE ItemsPmt_Cursor cursor for	
		--Select InvoiceKey From Invoice Where Status <7 and CompanyID = @Company 

		--FOR  READ ONLY
		--OPEN ItemsPmt_Cursor
		--FETCH NEXT FROM ItemsPmt_Cursor INTO 
		--@InvcKey
	
		--WHILE @@FETCH_STATUS = 0
		--BEGIN

		--iF((Select Count(*) From Invoice a Inner join [mas500_u_app].dbo.tapVoucher b on a.VendorKey =b.VendKey And a.CompanyID = b.CompanyID And a.Folio = b.TranNo Where a.InvoiceKey = @InvcKey)>=1)
		--BEGIN
		--Update Invoice		Set Status = 4 Where InvoiceKey = @invcKey
		--Update InvoiceLines Set Status = 4 Where InvoiceKey = @invcKey
		--IF @@ERROR <>0 GOTO Error_Capturado

		--	IF((Select Count(*) From Invoice a Inner Join InvcRcptDetails b on a.InvoiceKey = b.InvoiceKey Where a.InvoiceKey = @invcKey)>=1)
		--	BEGIN
		--	Update Invoice		Set Status = 5 Where InvoiceKey = @invcKey
		--	Update InvoiceLines Set Status = 5 Where InvoiceKey = @invcKey
		--	IF @@ERROR <>0 GOTO Error_Capturado
			
		--		   IF((Select Count(*) From Invoice a Inner Join InvcRcptDetails b on a.InvoiceKey = b.InvoiceKey inner join InvoiceReceipt c on b.InvcRcptKey = c.InvcRcptKey Where a.InvoiceKey = @invcKey)>=1)
		--		   BEGIN
		--			Update Invoice		Set Status = 6 Where InvoiceKey = @invcKey
		--			Update InvoiceLines Set Status = 6 Where InvoiceKey = @invcKey
		--			IF @@ERROR <>0 GOTO Error_Capturado
		--		   END -- CIERRA IF DE COMPARACION CON TABLA CONTRA RECIBOS
		--	END -- CIERRA IF DE COMPARACION CON TABLA CONTRA RECIBOS
		--END -- CIERRA IF DE COMPARACIÓN CON TABLA TAPVENDORS DE SAGE

		--FETCH NEXT FROM ItemsPmt_Cursor INTO 
		--@InvcKey
	
		--END -- CURSOR

		--CLOSE ItemsPmt_Cursor
		--DEALLOCATE ItemsPmt_Cursor
		Update A 
		SET A.status = 4
		From Invoice A
		inner join [mas500_u_app].dbo.tapVoucher b 
		on a.VendorKey = b.VendKey AND a.CompanyID = b.CompanyID And a.Folio = b.TranNo 
		Where a.Status = 2 And a.CompanyID = @Company
		IF @@ERROR <>0 GOTO Error_Capturado

		Update A 
		SET A.status = 5
		From Invoice A
		inner join InvcRcptDetails b 
		on a.InvoiceKey = b.InvoiceKey
		And a.Status = 4 And a.CompanyID = @Company
		IF @@ERROR <>0 GOTO Error_Capturado

		Update A 
		SET A.status = 6
		From Invoice A
		Inner Join InvcRcptDetails b on a.InvoiceKey = b.InvoiceKey 
		inner join InvoiceReceipt c on b.InvcRcptKey = c.InvcRcptKey 
		Inner join ChkReqDetail d on d.InvcRcptKey = c.InvcRcptKey
		Where a.Status = 5 And a.CompanyID = @Company
		IF @@ERROR <>0 GOTO Error_Capturado
			
		COMMIT TRAN

		Error_Capturado:
		IF @@ERROR <> 0
		BEGIN
		ROLLBACK
		INSERT INTO ErrorLog(ErrorKey,UserKey,LogKey,ErrorDate,proceso,Message,CompanyID) VALUES (1,1,1,GETDATE(),'spUpdateInvoice',ERROR_MESSAGE(),@Company)
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
			ROLLBACK TRAN	
			INSERT INTO ErrorLog(ErrorKey,UserKey,LogKey,ErrorDate,proceso,Message,CompanyID) VALUES (1,1,1,GETDATE(),'spUpdateInvoice',ERROR_MESSAGE(),@Company)			
			SELECT @ERROR_MESSAGE As Resultado
		END CATCH
		END




