USE [PortalProveedoresProd_Extra]
GO

/****** Object:  StoredProcedure [dbo].[UpdateSolCheq]    Script Date: 26/01/2023 8:41:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER Procedure [dbo].[UpdateSolCheq]
@UserKey INT,
@FacKey INT,
@Opcion INT
AS
BEGIN
Declare @Total int
Declare @key int
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
DECLARE @NIVEL INT
DECLARE @ANTERIOR INT
DECLARE @CONTKEY INT

IF (@Opcion = 1)
		BEGIN
		
		
		-- Registra en Tabla Users
		BEGIN TRY
			BEGIN TRAN

			SET @NIVEL = ( select top 1 a.RoleKey From UsersInRoles a inner join roles b on a.RoleKey = b.RoleKey Where UserKey = @UserKey)
			IF(@NIVEL = 7) -- Rol Validador
			BEGIN
				SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
				SET @NIVEL = 7
				--UPDATE a set Aprobador = Null From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
				UPDATE a set Rechazador = Null, Comment = NULL From InvoiceReceipt a Where a.InvcRcptKey = @CONTKEY
			END

			
			IF(@NIVEL = 13) -- Rol Tesoreria
			BEGIN
				SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
				UPDATE a set Aprobador = NULL, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
				IF @@ERROR <>0 GOTO Error_Capturado
				UPDATE approInvcReceipt set Nivel1 = NULL, Date1 = NULL, Generado = 0 where InvcRcptKey = @CONTKEY
				IF @@ERROR <>0 GOTO Error_Capturado
			END

			
			IF(@NIVEL = 14) -- Rol Finanzas
			BEGIN
				SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
				SET @ANTERIOR = (Select Top 1 Nivel1 From approInvcReceipt where InvcRcptKey = @CONTKEY)

				UPDATE a set Aprobador = @ANTERIOR, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
				IF @@ERROR <>0 GOTO Error_Capturado
				UPDATE approInvcReceipt set Nivel2 = NULL, Date2 = NULL, Generado = 0 where InvcRcptKey = @CONTKEY
				IF @@ERROR <>0 GOTO Error_Capturado
			END


			IF(@NIVEL = 5) -- Rol Admin
			BEGIN
				SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
				
				IF((Select Count(*) From approInvcReceipt where InvcRcptKey = @CONTKEY) = 0)
				BEGIN
					UPDATE a set Aprobador = NULL, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
					IF @@ERROR <>0 GOTO Error_Capturado
				END
				ELSE
				BEGIN
					
					IF((Select Count(*) From approInvcReceipt where InvcRcptKey = @CONTKEY AND Nivel1 IS NOT NULL AND Nivel2 IS NULL AND Nivel3 IS NULL) > 0)
					BEGIN
						UPDATE a set Aprobador = NULL, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
						IF @@ERROR <>0 GOTO Error_Capturado
						UPDATE approInvcReceipt set Nivel1 = NULL, Date1 = NULL, Generado = 0 where InvcRcptKey = @CONTKEY
						IF @@ERROR <>0 GOTO Error_Capturado
					END

					IF((Select Count(*) From approInvcReceipt where InvcRcptKey = @CONTKEY AND Nivel1 IS NOT NULL AND Nivel2 IS NOT NULL AND Nivel3 IS NULL) > 0)
					BEGIN
						SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
						SET @ANTERIOR = (Select Top 1 Nivel1 From approInvcReceipt where InvcRcptKey = @CONTKEY)

						UPDATE a set Aprobador = @ANTERIOR, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
						IF @@ERROR <>0 GOTO Error_Capturado
						UPDATE approInvcReceipt set Nivel2 = NULL, Date2 = NULL, Generado = 0 where InvcRcptKey = @CONTKEY
						IF @@ERROR <>0 GOTO Error_Capturado
					END


					IF((Select Count(*) From approInvcReceipt where InvcRcptKey = @CONTKEY AND Nivel1 IS NOT NULL AND Nivel2 IS NOT NULL AND Nivel3 IS NOT NULL) > 0)
					BEGIN
						SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
						SET @ANTERIOR = (Select Top 1 Nivel2 From approInvcReceipt where InvcRcptKey = @CONTKEY)

						UPDATE a set Aprobador = @ANTERIOR, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
						IF @@ERROR <>0 GOTO Error_Capturado
						UPDATE approInvcReceipt set Nivel3 = NULL, Date3 = NULL, Generado = 0 where InvcRcptKey = @CONTKEY
						IF @@ERROR <>0 GOTO Error_Capturado
					END
				END
				
			END



			COMMIT TRAN

		Error_Capturado:
		IF @@ERROR <> 0
		BEGIN
		ROLLBACK
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
			INSERT INTO ErrorLog(ErrorKey,UserKey,LogKey,ErrorDate,proceso,Message,CompanyID) VALUES (1,1,1,GETDATE(),'spUpdatePayment',ERROR_MESSAGE(),'TSM')			
		END CATCH
		END

IF (@Opcion = 2)
BEGIN
		
		
		-- Registra en Tabla Users
		BEGIN TRY
			BEGIN TRAN

			SET @NIVEL = ( select top 1 a.RoleKey From UsersInRoles a inner join roles b on a.RoleKey = b.RoleKey Where UserKey = @UserKey)
			
			IF(@NIVEL = 7) -- Rol Validador
			BEGIN
				SET @NIVEL = 7
				SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
				UPDATE a set Aprobador = @UserKey, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
				IF @@ERROR <>0 GOTO Error_Capturado2
				IF((Select Count(*) From approInvcReceipt where InvcRcptKey = @CONTKEY) = 0)
				BEGIN
					INSERT INTO approInvcReceipt (InvcRcptKey,Nivel1,Date1) Values (@CONTKEY,@UserKey,GETDATE())
					IF @@ERROR <>0 GOTO Error_Capturado2
				END
				ELSE
				BEGIN
					UPDATE approInvcReceipt set Nivel1 = @UserKey ,Date1 = GETDATE() where InvcRcptKey = @CONTKEY
					IF @@ERROR <>0 GOTO Error_Capturado2
				END
			END

			
			IF(@NIVEL = 13) -- Rol Tesoreria
			BEGIN
				SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
				UPDATE a set Aprobador = @UserKey, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
				IF @@ERROR <>0 GOTO Error_Capturado2
				UPDATE approInvcReceipt set Nivel2 = @UserKey, Date2 = GETDATE() where InvcRcptKey = @CONTKEY
				IF @@ERROR <>0 GOTO Error_Capturado2
			END

			
			IF(@NIVEL = 14) -- Rol Finanzas
			BEGIN
				SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
				SET @ANTERIOR = (Select Top 1 Nivel1 From approInvcReceipt where InvcRcptKey = @CONTKEY)

				UPDATE a set Aprobador = @UserKey, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
				IF @@ERROR <>0 GOTO Error_Capturado2
				UPDATE approInvcReceipt set Nivel3 = @UserKey, Date3 = GETDATE(), Generado = 1 where InvcRcptKey = @CONTKEY
				IF @@ERROR <>0 GOTO Error_Capturado2
			END


			IF(@NIVEL = 5) -- Rol Admin
			BEGIN
				SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
				
				IF((Select Count(*) From approInvcReceipt where InvcRcptKey = @CONTKEY) = 0)
				BEGIN
						UPDATE a set Aprobador = @UserKey, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
						IF @@ERROR <>0 GOTO Error_Capturado2
						INSERT INTO approInvcReceipt (InvcRcptKey,Nivel1,Date1) Values (@CONTKEY,@UserKey,GETDATE())
						IF @@ERROR <>0 GOTO Error_Capturado2
				END
				ELSE
				BEGIN

					IF((Select Count(*) From approInvcReceipt where InvcRcptKey = @CONTKEY AND Nivel1 IS NOT NULL AND Nivel2 IS NOT NULL AND Nivel3 IS NULL) > 0)
					BEGIN
						SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
						SET @ANTERIOR = (Select Top 1 Nivel1 From approInvcReceipt where InvcRcptKey = @CONTKEY)

						UPDATE a set Aprobador = @UserKey, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
						IF @@ERROR <>0 GOTO Error_Capturado2
						UPDATE approInvcReceipt set Nivel3 = @UserKey, Date3 = GETDATE(), Generado = 1  where InvcRcptKey = @CONTKEY
						IF @@ERROR <>0 GOTO Error_Capturado2
					END

					IF((Select Count(*) From approInvcReceipt where InvcRcptKey = @CONTKEY AND Nivel1 IS NOT NULL AND Nivel2 IS NULL AND Nivel3 IS NULL) > 0)
					BEGIN
						UPDATE a set Aprobador = @UserKey, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
						IF @@ERROR <>0 GOTO Error_Capturado2
						UPDATE approInvcReceipt set Nivel2 = @UserKey, Date2 = GETDATE() where InvcRcptKey = @CONTKEY
						IF @@ERROR <>0 GOTO Error_Capturado2
					END

					IF((Select Count(*) From approInvcReceipt where InvcRcptKey = @CONTKEY AND Nivel1 IS NULL AND Nivel2 IS NULL AND Nivel3 IS NULL) > 0)
					BEGIN
						UPDATE a set Aprobador = @UserKey, Rechazador = Null, Comment = NULL From InvoiceReceipt a inner join InvcRcptDetails b on a.InvcRcptKey = b.InvcRcptKey  Where b.InvoiceKey = @FacKey
						IF @@ERROR <>0 GOTO Error_Capturado2
						UPDATE approInvcReceipt set Nivel1 = @UserKey, Date1 = GETDATE() where InvcRcptKey = @CONTKEY
						IF @@ERROR <>0 GOTO Error_Capturado2
					END

				END
				
			END



			COMMIT TRAN

		Error_Capturado2:
		IF @@ERROR <> 0
		BEGIN
		ROLLBACK
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
			INSERT INTO ErrorLog(ErrorKey,UserKey,LogKey,ErrorDate,proceso,Message,CompanyID) VALUES (1,1,1,GETDATE(),'spUpdatePayment',ERROR_MESSAGE(),'TSM')			
		END CATCH
		END

IF(@Opcion = 3)
	BEGIN
	Declare @Folio INT
	DECLARE ItemsPmt_Cursor cursor for	
	Select ApplInvoiceKey From PaymentAppl Where PaymentKey = @FacKey

	FOR  READ ONLY
	OPEN ItemsPmt_Cursor
	FETCH NEXT FROM ItemsPmt_Cursor INTO 
	@Folio
	
    WHILE @@FETCH_STATUS = 0
	BEGIN

	Declare @Pagos Decimal(18,6)
	Declare @Saldo Decimal(18,6)
	SET @Pagos = (Select IsNull(SUM(PaymtApplied),0) As Total From PaymentAppl a inner join Payment b on a.PaymentKey = b.PaymentKey Where ApplInvoiceKey = @Folio and b.Status =2)
	SET @Saldo = (Select Total From Invoice Where InvoiceKey = @Folio)
	IF (@Pagos >= @Saldo)
	BEGIN
		UPDATE Invoice SET Status = 9, UpdateUserKey = @UserKey , UpdateDate = @Hoy Where InvoiceKey = @Folio 
	END

	FETCH NEXT FROM ItemsPmt_Cursor INTO 
    @Folio
	
	END -- CURSOR

	CLOSE ItemsPmt_Cursor
	DEALLOCATE ItemsPmt_Cursor
END


END





GO

