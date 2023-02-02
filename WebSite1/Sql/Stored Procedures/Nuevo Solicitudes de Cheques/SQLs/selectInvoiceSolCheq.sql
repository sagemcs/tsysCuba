USE [PortalProveedoresProd_Extra]
GO

/****** Object:  StoredProcedure [dbo].[spSelectInvoiceSolCheq]    Script Date: 20/01/2023 8:11:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER proc [dbo].[spSelectInvoiceSolCheq]
@VendId varchar(255),
@Folio varchar(40),
@FechaRec varchar(36),
@FolioC varchar(40),
@FechaApr varchar(36),
@FechaProg varchar(36),
@Total varchar(12),
@UserKey int,
@CompanyID varchar(3)
AS
BEGIN

DECLARE @ERRORGEN INT
DECLARE @IDERROR INT		
DECLARE @ERROR_MESSAGE		NVARCHAR(2048)
DECLARE @ERROR_NUMBER		INT
DECLARE @ERROR_PROCEDURE	NVARCHAR(126)
DECLARE @DB_NAME			NVARCHAR(128)
DECLARE @ERROR_LINE			INT
DECLARE @ERROR_SEVERITY		INT
DECLARE @FILTRO Varchar(max)
DECLARE @VendKey int
DECLARE @SQLString VARCHAR(MAX)
DECLARE @NIVEL INT


BEGIN TRY	

SET @FILTRO = ' AND t4.CompanyID = ''' + @CompanyID + ''''

	IF LEN(@VendId) > 0
	BEGIN
	   SET @FILTRO = @FILTRO + ' AND d.vendName like ''' + '%'+ @VendId + + '%' + ''''
	END

	IF LEN(@Folio) > 0
	BEGIN
	   SET @FILTRO = @FILTRO + ' AND t4.Folio like ''' + '%'+ @Folio + + '%' + ''''
	END

	IF LEN(@FechaRec) > 0
	BEGIN
	   SET @FILTRO = @FILTRO + ' AND CONVERT(DATE,t4.UpdateDate)  >= ''' + @FechaRec + '''' + ' AND CONVERT(DATE,t4.UpdateDate) <= ''' + @FechaRec + ''''
	END

	IF LEN(@FolioC) > 0
	BEGIN
	   SET @FILTRO = @FILTRO + ' AND t1.Folio like ''' + '%'+ @FolioC + + '%' + ''''
	END

	IF LEN(@FechaApr) > 0
	BEGIN
	   SET @FILTRO = @FILTRO + ' AND CONVERT(DATE,t4.AprovDate)  >= ''' + @FechaApr + '''' + ' AND CONVERT(DATE,t4.AprovDate) <= ''' + @FechaApr + ''''
	END

	IF LEN(@FechaProg) > 0
	BEGIN
	   SET @FILTRO = @FILTRO + ' AND CONVERT(DATE,t1.PaymentDate)  >= ''' + @FechaProg + '''' + ' AND CONVERT(DATE,t1.PaymentDate) <= ''' + @FechaProg + ''''
	END

	IF LEN(@Total) > 0
	BEGIN
	   SET @FILTRO = @FILTRO + ' AND t4.Total like ''' + '%'+ @Total + + '%' + ''''
	END

	SET @NIVEL = ( select top 1 a.RoleKey From UsersInRoles a inner join roles b on a.RoleKey = b.RoleKey Where UserKey = @UserKey)

	IF(@NIVEL = 7) -- Rol Validador
	BEGIN
		SET @FILTRO = @FILTRO + ' AND b1.Nivel1 IS NULL AND b1.Nivel2 IS NULL  AND b1.Nivel3 IS NULL '
	END

	IF(@NIVEL = 13) -- Rol Tesoreria
	BEGIN
		SET @FILTRO = @FILTRO + ' AND b1.Nivel1 IS NOT NULL AND b1.Nivel2 IS  NULL  AND b1.Nivel3 IS NULL  AND t1.Rechazador IS NULL '
	END

	IF(@NIVEL = 14) -- Rol Finanzas
	BEGIN
		SET @FILTRO = @FILTRO + ' AND b1.Nivel1 IS NOT NULL AND b1.Nivel2 IS NOT NULL  AND b1.Nivel3 IS NULL AND t1.Rechazador IS NULL'
	END

	SET @SQLString =  N'SELECT t4.InvoiceKey,d.vendName as Vendor,a.VendDBA as RFC ,c.PmtTermsID as Terminos,t4.Folio + ''-'' + t4.Serie As FolioF, t4.UpdateDate as FechaR , t1.Folio as FolioC , t4.AprovDate As FechaAP , t1.PaymentDate As FechaPago, t4.Total as Total,b1.Nivel1,Nivel2,Nivel3 , ISNULL(t5.username + '' - '' + t7.roleID,NULL) as Aprobador'
	SET @SQLString = @SQLString +  ' FROM InvoiceReceipt t1 LEFT JOIN approInvcReceipt b1 on b1.InvcRcptKey = t1.InvcRcptKey '
	SET @SQLString = @SQLString +  ' LEFT JOIN deniedInvoiceReceipt dir on t1.InvcRcptKey = dir.InvcRcptKey '
	SET @SQLString = @SQLString +  ' LEFT JOIN invcRcptDetails t3 on t1.InvcRcptKey = t3.InvcRcptKey '
	SET @SQLString = @SQLString +  ' LEFT JOIN invoice t4 on t3.InvoiceKey = t4.InvoiceKey '
	SET @SQLString = @SQLString +  ' LEFT JOIN Vendors d on t1.VendorKey = d.VendorKey '
	SET @SQLString = @SQLString +  ' LEFT JOIN [sage500_Portal081122].dbo.TapVendor a ON a.VendKey = d.VendorKey  '
	SET @SQLString = @SQLString +  ' LEFT JOIN [sage500_Portal081122].dbo.tciPaymentTerms c ON a.PmtTermsKey = c.PmtTermsKey '
	SET @SQLString = @SQLString +  ' LEFT JOIN users t5 on t1.Aprobador = t5.userkey '  
	SET @SQLString = @SQLString +  ' LEFT JOIN users t8 on t1.Rechazador = t8.userkey '
	SET @SQLString = @SQLString +  ' LEFT JOIN usersinroles t6 on t5.userkey = t6.userkey  ' 
	SET @SQLString = @SQLString +  ' LEFT JOIN roles t7 on t6.rolekey = t7.rolekey'
	SET @SQLString = @SQLString +  ' WHERE NOT EXISTS(SELECT NULL FROM ChkReqDetail t2 WHERE t2.InvcRcptKey = t1.InvcRcptKey)'
	SET @SQLString = @SQLString + @FILTRO ;
	--SELECT @SQLString
	EXEC(@SQLString)
	
END TRY
BEGIN CATCH
		DECLARE @EMPRESA VARCHAR(225)
		SET @ERRORGEN = 1			
		SET @ERROR_MESSAGE = ERROR_MESSAGE()
		SET @ERROR_NUMBER = ERROR_NUMBER()
		SET @ERROR_PROCEDURE = ERROR_PROCEDURE()
		SET @DB_NAME = DB_NAME()
		SET @ERROR_LINE = ERROR_LINE()
		SET @ERROR_SEVERITY = ERROR_SEVERITY()
		SET @EMPRESA = NULL

		Select -1

END CATCH

END



GO

