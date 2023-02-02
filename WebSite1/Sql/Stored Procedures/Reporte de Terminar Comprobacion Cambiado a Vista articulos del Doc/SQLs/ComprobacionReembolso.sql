USE [PortalProveedoresProd_Extra]
GO

/****** Object:  StoredProcedure [dbo].[SpReportComprobacionGastosReembolso]    Script Date: 25/01/2023 9:30:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		Manuel Barreiro 
-- Create date: 25/01/2023
-- Description:	Comprobacion de Gastos de Reembolso
-- =============================================
 CREATE   PROCEDURE [dbo].[SpReportComprobacionGastosReembolso]
	-- Add the parameters for the stored procedure here
	@docKey int,
	@createUser int,
	@companyId varchar(MAX)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT DetailId,ExpenseId,ItemKey,Qty,UnitCost, STaxCodeKey,TaxAmount, Type 
	FROM ExpenseDetail 
	where ExpenseId = @docKey and CreateUser = @createUser and CompanyId = @CompanyId;
END
GO

