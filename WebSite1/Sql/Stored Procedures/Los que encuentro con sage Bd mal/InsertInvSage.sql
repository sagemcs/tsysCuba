USE [PortalProveedoresProd_Error]
GO
/****** Object:  StoredProcedure [dbo].[spapiInsertInvSage]    Script Date: 10/02/2023 4:28:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[spapiInsertInvSage]
(   
@Invckey int,
@VendKey int,
@UserID varchar(50),
@CompanyID varchar(50),
@oIDBitacora INT OUTPUT,
@ovKey INT OUTPUT,
@oRetVal INT OUTPUT
)
AS
BEGIN
BEGIN TRY

DECLARE @TipoRegistro	varchar(5)
DECLARE @NumVoucher	  varchar(13)
DECLARE @POTranID	 varchar(13)
DECLARE @VendID      varchar(12)
DECLARE @Timbrado bit
DECLARE @RFC	     varchar(13)
DECLARE @FechaVoucher	datetime
DECLARE @MetodoPago	varchar(3)
DECLARE @FormaPago	varchar(2)
DECLARE @CurrID	varchar(3)
DECLARE @UsoCFDI	varchar(3)
DECLARE @Subtotal	decimal(15,3)
DECLARE @Total	decimal(15,3)
DECLARE @Descuento decimal(15,3)
DECLARE @Impuestos	decimal(15,3)
DECLARE @UUID	varchar(36)
DECLARE @Batchkey	int

DECLARE @VendID2  varchar(12)
DECLARE @VendRefNo varchar(15)
DECLARE @TaxPayerID varchar(30)
DECLARE @VendDBA varchar(30)
DECLARE @status int

DECLARE @TranID VARCHAR(13)
DECLARE @TranNo VARCHAR(10)
DECLARE @TranDate DATETIME
DECLARE @STaxAmt DECIMAL(15,3)
DECLARE @TranAmt DECIMAL(15,3)
DECLARE @TranAmtHC DECIMAL(15,3)

DECLARE @Itemkey INT
DECLARE @description VARCHAR(40)
DECLARE @status2 int
DECLARE @qtyord DECIMAL(15,3)
DECLARE @unitcost DECIMAL(15,3)
DECLARE @unitmeaskey int
DECLARE @ExtAmt  DECIMAL(15,3)
declare @VendAddrKey int
declare @Comentario int
declare @Carga int
declare @NombreArchivo varchar(800)
declare @DescError	varchar(100)
declare @Error	int	
declare @eliminado	bit
declare @FechaCreacion	datetime
declare @FechaModificado	datetime
declare @vKey int
DECLARE @PoKey2 int
DECLARE @TaxID VARCHAR(13)
DECLARE @IDBitacora INT


-------------------
SET @IDBitacora = 0
--------------------
SET @TipoRegistro = 401

SET @Carga = 0
SET @Error =  0
SET @eliminado = 0
SET @status  =0
--SET @FechaCreacion = GETDATE()
SET @FechaModificado = GETDATE()
--------------------

        --CONSULTAR ENCABEZADO FACTURA		
		SELECT @RFC=RFCEmisor,@Timbrado=timbrado,@POTranID=NodeOc,@FechaVoucher=FechaTransaccion,@UUID=UUID,@Descuento=descuento,
		@NumVoucher=Folio,@MetodoPago=MetodoPago,@FormaPago=FormaPago,@UsoCFDI=UsoCFDI,@CurrID=Moneda,@timbrado=timbrado,@FechaCreacion=UpdateDate,
		@Subtotal=Subtotal,@Impuestos=ImpuestoImporteTrs,@Total=Total
		FROM Invoice
		where InvoiceKey=@Invckey and Vendorkey = @VendKey and  CompanyID = @CompanyID		

		SELECT @VendID2=VendID,@VendRefNo=VendRefNo,@TaxPayerID=TaxPayerID,@VendDBA=VendDBA,@status=Status,@VendAddrKey=DfltPurchAddrKey
		FROM [mas500_u_app].dbo.tapVendor WHERE Vendkey= @Vendkey AND CompanyID = @CompanyID

		 --CONSULTAR OC
		 SELECT @PoKey2=PoKey FROM [mas500_u_app].dbo.tpoPurchOrder where TranNo = @POTranID and  CompanyID = @CompanyID

	     exec spapiInsertBitacora @POTranID,@VendID2,@UserID,@CompanyID,@IDBitacora OUTPUT,@oRetVal OUTPUT
	 
	     exec spapiInsertStgFPA @IDBitacora,@TipoRegistro,@NumVoucher,@POTranID,@VendKey,@VendAddrKey,
								@RFC,@FechaVoucher,@MetodoPago,@FormaPago,@CurrID,@UsoCFDI,@Subtotal,@Descuento,@Total,@Impuestos,@UUID,@timbrado,@Comentario,
								@Batchkey,@Carga,@NombreArchivo,@DescError,@Error,@eliminado,@status,@FechaCreacion,@FechaModificado,@UserID,@CompanyID,@vKey OUTPUT, @oRetVal OUTPUT
		  
        IF @oRetVal = 1

		BEGIN
	    DECLARE @dtlKey int
		DECLARE @ItemID VARCHAR(30)
		DECLARE @ItemIDSAT VARCHAR(10)
		DECLARE @Qty decimal(16,8)
		DECLARE @UnitCostDtl decimal(16,8)
		DECLARE @TranAmtDtl decimal(16,8)
		DECLARE @DescuentoDtl decimal(16,8)
		DECLARE @Valido int
		declare @Rate decimal(15, 3)
		Declare @noPartida int
				
			DECLARE ProdInfo CURSOR FOR 
			SELECT InvoiceLineKey,Codigo,ClaveProd,Cantidad,ValorUnitario,Importe,Descuento,Unidad
			FROM InvoiceLines WHERE InvoiceKey =@Invckey
			
			OPEN ProdInfo

			FETCH NEXT FROM ProdInfo INTO @dtlKey,@ItemID,@ItemIDSAT,@Qty,@UnitCostDtl,@TranAmtDtl,@DescuentoDtl,@noPartida

			WHILE @@fetch_status = 0
			BEGIN

				       set @Valido= 0
					
					   exec spapiInsertStgFPADt @IDBitacora,@vKey,@Invckey,@dtlKey,@TipoRegistro,@ItemID,@ItemIDSAT,@Qty,@UnitCostDtl,@TranAmtDtl,@DescuentoDtl,
					   @noPartida,@DescError,@Error,@eliminado,@status,@FechaCreacion,@FechaModificado,@UserID,@CompanyID,@oRetVal

					   IF @oRetVal = -1
					   BEGIN

					    SET @oRetVal = @oRetVal
						SET @oIDBitacora = @IDBitacora
						SET @ovKey= @vKey

						RETURN

					   END


				FETCH NEXT FROM ProdInfo INTO @dtlKey,@ItemID,@ItemIDSAT,@Qty,@UnitCostDtl,@TranAmtDtl,@DescuentoDtl,@noPartida
			END

			CLOSE ProdInfo
			DEALLOCATE ProdInfo
		END

			SET @oRetVal = @oRetVal
			SET @oIDBitacora = @IDBitacora
			SET @ovKey= @vKey
            
END TRY
BEGIN CATCH
		
	  INSERT INTO [mas500_u_app].dbo.tapAPILogValidacion(iLote,vKey,dtlKey,fechaError,ErrorValidacion,Proceso,eliminado,CompanyID)
	  VALUES (@IDBitacora,0,0,GETDATE(),ERROR_MESSAGE(),'Carga Inicial-Lote',0,@CompanyID)

	SET @oIDBitacora = -1
	SET @ovKey= -1
	  SET @oRetVal = -1 -- ERROR	
	  	
END CATCH
END




