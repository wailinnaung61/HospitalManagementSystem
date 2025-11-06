-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zun Pwint Phyu
-- Create date: 2022 July,28
-- Description:	Get LabOrder from LabOrderTest
-- =============================================
Alter PROCEDURE SP_GetLabOrderFromLabOrderTest
	-- Add the parameters for the stored procedure here
	@BranchId int=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select l.*,'' as PatientName,'' as PatientGuardian,'' as BranchName,'' as BranchAddress,'' as BranchPhone from LabOrder l 
	join LabOrderTest lt on lt.LabOrderId=l.Id where lt.LabResultId is null and l.IsPaid=0 and l.IsDelete=0 and
	(@BranchId is null or l.BranchId=@BranchId)
END
GO
