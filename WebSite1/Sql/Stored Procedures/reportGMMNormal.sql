USE [PortalProveedoresProd_Error]
GO

/****** Object:  StoredProcedure [dbo].[SpReportMinorMedicalExpense]    Script Date: 02/02/2023 6:06:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		Manuel Barreiro 
-- Create date: 16-08-2022
-- Description:	Reporte gastos medicos menores
-- =============================================
ALTER   PROCEDURE [dbo].[SpReportMinorMedicalExpense]
	-- Add the parameters for the stored procedure here
	@UpdateUserKey int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select ISNULL(convert(varchar, Date, 3) , '') as 'Fecha', Amount as 'Importe', Status as 'Estado'
	from MinorMedicalExpense
	where UpdateUserKey = @UpdateUserKey
END

GO

