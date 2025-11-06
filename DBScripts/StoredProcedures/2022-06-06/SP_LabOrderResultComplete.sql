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
-- Create date: 2022,June 06
-- Description:	<Description,,>
-- =============================================
Alter PROCEDURE SP_LabOrderResultComplete
	-- Add the parameters for the stored procedure here
	@BranchId int=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select O.*,P.Name AS [PatientName], P.Guardian AS [PatientGuardian], B.Name as BranchName,B.Address AS [BranchAddress],B.Phone AS [BranchPhone] from LabOrder O 
	LEFT JOIN Patient P ON O.PatientId=P.Id
		LEFT JOIN Branch B ON O.BranchId=B.Id
left join LabOrderTest OT on O.Id=OT.LabOrderId
left join LabResult R on R.Id=OT.LabResultId
where R.IsCompleted=1
and (@BranchId is null Or O.BranchId=@BranchId)
and O.IsDelete=0
END
GO
