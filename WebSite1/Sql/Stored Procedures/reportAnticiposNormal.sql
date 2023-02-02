USE [PortalProveedoresProd_Error]
GO

/****** Object:  StoredProcedure [dbo].[SpReportAnticipoGastos]    Script Date: 02/02/2023 6:02:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





-- =============================================
-- Author:		Manuel Barreiro 
-- Create date: 16-08-2022
-- Description:	Reporte anticipo de gastos
-- =============================================
ALTER   PROCEDURE [dbo].[SpReportAnticipoGastos]
	-- Add the parameters for the stored procedure here
	@UpdateUserKey int,
	@docKey int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF(@docKey <> -1)
	BEGIN
		-- Insert statements for procedure here
		select AdvanceType as 'Tipo', Amount as 'Importe', ISNULL(convert(varchar, DepartureDate, 3) , '') as 'FechaSalida', ISNULL(convert(varchar, ArrivalDate, 3) , '') as 'FechaLLegada', 
				ISNULL(convert(varchar, CheckDate, 3) , '') as 'FechaComprobacion', ImmediateBoss as 'JefeInmediato', Status as 'Estado'
		from Advance
		where UpdateUserKey = @UpdateUserKey and AdvanceId = @docKey
	END

	IF(@docKey = -1)
	BEGIN
		-- Insert statements for procedure here
		select AdvanceType as 'Tipo', Amount as 'Importe', ISNULL(convert(varchar, DepartureDate, 3) , '') as 'FechaSalida', ISNULL(convert(varchar, ArrivalDate, 3) , '') as 'FechaLLegada', 
				ISNULL(convert(varchar, CheckDate, 3) , '') as 'FechaComprobacion', ImmediateBoss as 'JefeInmediato', Status as 'Estado'
		from Advance
		where UpdateUserKey = @UpdateUserKey
	END
    
END

GO

