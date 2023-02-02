USE [PortalProveedoresProd_Extra]
GO

/****** Object:  StoredProcedure [dbo].[DenyInvoiceReceipt]    Script Date: 20/01/2023 8:09:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DenyInvoiceReceipt]
	-- Add the parameters for the stored procedure here
	@UserKey INT,
	@FacKey INT,
	@Motivo varchar(MAX)
AS
BEGIN
	DECLARE @CONTKEY INT
	DECLARE @ERRORGEN INT
	DECLARE @ERROR_MESSAGE		NVARCHAR(2048)
	DECLARE @ERROR_NUMBER		INT
	DECLARE @ERROR_PROCEDURE	NVARCHAR(126)
	DECLARE @DB_NAME			NVARCHAR(128)
	DECLARE @ERROR_LINE			INT
	DECLARE @ERROR_SEVERITY		INT	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
	   BEGIN
			SET @CONTKEY = (Select top 1 b.InvcRcptKey From InvcRcptDetails a inner join InvoiceReceipt b on a.InvcRcptKey = b.InvcRcptKey Where a.InvoiceKey = @FacKey)
			UPDATE a set Rechazador = @UserKey, Comment = @Motivo, Status = 3 From InvoiceReceipt a Where a.InvcRcptKey = @CONTKEY
			IF @@ERROR <>0 GOTO Error_Capturado
			DELETE FROM approInvcReceipt where InvcRcptKey = @CONTKEY
			--UPDATE approInvcReceipt set Nivel1 = NULL, Date1 = NULL, Generado = 0 where InvcRcptKey = @CONTKEY
			IF @@ERROR <>0 GOTO Error_Capturado
			INSERT INTO deniedInvoiceReceipt(InvcRcptKey, FechaRechazo) values (@CONTKEY, GETDATE())
			IF @@ERROR <>0 GOTO Error_Capturado
		END

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
			INSERT INTO ErrorLog(ErrorKey,UserKey,LogKey,ErrorDate,proceso,Message,CompanyID) VALUES (1,1,1,GETDATE(),'DenyInvoiceReceipt',ERROR_MESSAGE(),'TSM')			
		END CATCH
END
GO

