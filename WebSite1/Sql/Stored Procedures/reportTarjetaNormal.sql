USE [PortalProveedoresProd_Error]
GO

/****** Object:  StoredProcedure [dbo].[SpReportTarjetaEmpleado]    Script Date: 02/02/2023 6:05:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		Manuel Barreiro 
-- Create date: 16-08-2022
-- Description:	Reporte tarjetas empleados
-- =============================================
ALTER   PROCEDURE [dbo].[SpReportTarjetaEmpleado]
	-- Add the parameters for the stored procedure here
	@UpdateUserKey int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here	
	SELECT        dbo.CorporateCardDetail.Type as 'Tipo', ISNULL(convert(varchar, dbo.CorporateCard.Date, 3) , '')  as 'Fecha', dbo.CorporateCard.Currency as 'TipoMoneda', sum(dbo.CorporateCardDetail.TaxAmount + dbo.CorporateCardDetail.Amount) as 'Importe', Status as 'Estado'
	FROM            dbo.CorporateCard INNER JOIN
                         dbo.CorporateCardDetail ON dbo.CorporateCard.CorporateCardId = dbo.CorporateCardDetail.CorporateCardId
	where dbo.CorporateCard.UpdateUserKey = @UpdateUserKey
						 group by Type, Date, Currency, Status

	
END

GO

