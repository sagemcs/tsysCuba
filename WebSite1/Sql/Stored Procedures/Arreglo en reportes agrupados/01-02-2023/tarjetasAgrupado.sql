USE [PortalProveedoresProd_Error]
GO

/****** Object:  StoredProcedure [dbo].[SpReportTarjetaEmpleadoGrouped]    Script Date: 02/02/2023 2:15:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		Manuel Barreiro 
-- Create date: 18/08/2022
-- Description:	Tabla Pivote - Reporte tarjeta empleado
-- =============================================
ALTER   PROCEDURE [dbo].[SpReportTarjetaEmpleadoGrouped]
	-- Add the parameters for the stored procedure here
	@UpdateUserKey int	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select 	ISNULL(convert(varchar, Date, 3) , '') as 'Fecha' ,  
	dbo.get_article_amount_by_user(f.Date,1,f.UpdateUserKey,Type, 1, f.CorporateCardId) as 'Aereo',
	dbo.get_article_amount_by_user(f.Date,2,f.UpdateUserKey,Type, 1, f.CorporateCardId) as 'Terrestre',
	dbo.get_article_amount_by_user(f.Date,3,f.UpdateUserKey,Type, 1, f.CorporateCardId) as 'Casetas',
	dbo.get_article_amount_by_user(f.Date,4,f.UpdateUserKey,Type, 1, f.CorporateCardId) as 'Gasolina',
	dbo.get_article_amount_by_user(f.Date,5,f.UpdateUserKey,Type, 1, f.CorporateCardId) as 'Estacionamiento',
	dbo.get_article_amount_by_user(f.Date,6,f.UpdateUserKey,Type, 1, f.CorporateCardId) as 'Alimentos',
	dbo.get_article_amount_by_user(f.Date,7,f.UpdateUserKey,Type, 1, f.CorporateCardId) as 'Hospedaje',
	dbo.get_article_amount_by_user(f.Date,8,f.UpdateUserKey,Type, 1, f.CorporateCardId) as 'GExtra'
	from 
	(
	SELECT       dbo.CorporateCardDetail.Type, dbo.CorporateCard.Date, dbo.CorporateCardDetail.Amount + dbo.CorporateCardDetail.TaxAmount AS Amount, UpdateUserKey, dbo.CorporateCardDetail.CorporateCardId
	FROM            dbo.CorporateCard INNER JOIN  dbo.CorporateCardDetail ON dbo.CorporateCard.CorporateCardId = dbo.CorporateCardDetail.CorporateCardId 
	where dbo.CorporateCard.UpdateUserKey = @UpdateUserKey) as f
END

GO

