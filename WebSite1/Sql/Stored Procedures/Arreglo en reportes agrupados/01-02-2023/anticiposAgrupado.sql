USE [PortalProveedoresProd_Error]
GO

/****** Object:  StoredProcedure [dbo].[SpReportAnticiposGastosGrouped]    Script Date: 02/02/2023 2:07:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





-- =============================================
-- Author:		Manuel Barreiro 
-- Create date: 17/08/2022
-- Description:	Tabla Pivote - Reporte anticipos de gastos
-- =============================================
 ALTER   PROCEDURE [dbo].[SpReportAnticiposGastosGrouped]
	-- Add the parameters for the stored procedure here
	@UpdateUserKey int	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select 	ISNULL(convert(varchar, f.DepartureDate, 3) , '') as 'FechaSalida',  
	ISNULL(convert(varchar, f.ArrivalDate, 3) , '') as 'FechaLLegada', 
	ISNULL(convert(varchar, CheckDate, 3) , '') as 'FechaComprobacion', 
	dbo.get_article_amount_by_user(f.DepartureDate,1,f.UpdateUserKey,AdvanceType, 3, f.AdvanceId) as 'Viaje',
	dbo.get_article_amount_by_user(f.DepartureDate,2,f.UpdateUserKey,AdvanceType, 3, f.AdvanceId) as 'GExtra'
	from 
	(
	SELECT       AdvanceType, DepartureDate, ArrivalDate, CheckDate, sum(Amount) as Amount, UpdateUserKey, AdvanceId
	FROM            Advance
	where UpdateUserKey = @UpdateUserKey
	group by AdvanceType, DepartureDate, ArrivalDate, CheckDate, UpdateUserKey, AdvanceId) as f
END

GO

