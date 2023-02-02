USE [PortalProveedoresProd_Error]
GO

/****** Object:  UserDefinedFunction [dbo].[get_article_amount_by_user]    Script Date: 02/02/2023 2:06:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[get_article_amount_by_user]
(
	-- Add the parameters for the function here
	@date datetime, @type int, @userkey int, @localType int, @documentType int, @id int
)
RETURNS decimal(17,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @retorno decimal(17,2)

	if @documentType = 1
		-- Add the T-SQL statements to compute the return value here
		if @type = @localType
			begin
				set @retorno = (SELECT dbo.CorporateCardDetail.TaxAmount + dbo.CorporateCardDetail.Amount AS Amount
					FROM            dbo.CorporateCard INNER JOIN  dbo.CorporateCardDetail ON dbo.CorporateCard.CorporateCardId = dbo.CorporateCardDetail.CorporateCardId 
					where dbo.CorporateCard.UpdateUserKey = @userkey and dbo.CorporateCardDetail.Type = @type and dbo.CorporateCard.Date = @date and dbo.CorporateCard.CorporateCardId = @id)
			end
	if @documentType = 2
		-- Add the T-SQL statements to compute the return value here
		if @type = @localType
			begin
				set @retorno = (SELECT dbo.ExpenseDetail.Amount + dbo.ExpenseDetail.TaxAmount AS Amount
					FROM            dbo.Expense INNER JOIN  dbo.ExpenseDetail ON dbo.Expense.ExpenseId = dbo.ExpenseDetail.ExpenseId 
					where dbo.Expense.UpdateUserKey = @userkey and dbo.ExpenseDetail.Type = @type and dbo.Expense.Date = @date and dbo.Expense.ExpenseId = @id)
			end
	if @documentType = 3
		-- Add the T-SQL statements to compute the return value here
		if @type = @localType
			begin
				set @retorno = (SELECT dbo.Advance.Amount AS Amount
					FROM            dbo.Advance
					where dbo.Advance.UpdateUserKey = @userkey and dbo.Advance.AdvanceId = @id)
			end
	-- Return the result of the function
	RETURN isnull(@retorno,0)

END

GO

