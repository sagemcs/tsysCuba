USE [PortalProveedoresProd_Error]
GO

/****** Object:  StoredProcedure [dbo].[SpReportComprobacionGastos]    Script Date: 02/02/2023 2:14:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





-- =============================================
-- Author:		Rafael Boza 
-- Create date: 26-07-2022
-- Description:	Tabla Pivote - Reporte comprobacion de gastos
-- =============================================
ALTER   PROCEDURE [dbo].[SpReportComprobacionGastos]
	-- Add the parameters for the stored procedure here
	@UpdateUserKey int	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select 	ISNULL(convert(varchar, Date, 3) , '') as 'Fecha' ,  
	dbo.get_article_amount_by_user(f.Date,1,f.UpdateUserKey,Type, 2, f.ExpenseId) as 'Aereo',
	dbo.get_article_amount_by_user(f.Date,2,f.UpdateUserKey,Type, 2, f.ExpenseId) as 'Terrestre',
	dbo.get_article_amount_by_user(f.Date,3,f.UpdateUserKey,Type, 2, f.ExpenseId) as 'Casetas',
	dbo.get_article_amount_by_user(f.Date,4,f.UpdateUserKey,Type, 2, f.ExpenseId) as 'Gasolina',
	dbo.get_article_amount_by_user(f.Date,5,f.UpdateUserKey,Type, 2, f.ExpenseId) as 'Estacionamiento',
	dbo.get_article_amount_by_user(f.Date,6,f.UpdateUserKey,Type, 2, f.ExpenseId) as 'Alimentos',
	dbo.get_article_amount_by_user(f.Date,7,f.UpdateUserKey,Type, 2, f.ExpenseId) as 'Hospedaje',
	dbo.get_article_amount_by_user(f.Date,8,f.UpdateUserKey,Type, 2, f.ExpenseId) as 'GExtra'
	from 
	(
	SELECT       dbo.ExpenseDetail.Type, dbo.Expense.Date, dbo.ExpenseDetail.Amount + dbo.ExpenseDetail.TaxAmount AS Amount, UpdateUserKey, dbo.Expense.ExpenseId
	FROM            dbo.Expense INNER JOIN  dbo.ExpenseDetail ON dbo.Expense.ExpenseId = dbo.ExpenseDetail.ExpenseId 
	where dbo.Expense.UpdateUserKey = @UpdateUserKey) as f
END

GO

