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
alter PROCEDURE SP_GetIPDRecordForDashboard
	-- Add the parameters for the stored procedure here
	@BranchId int =null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select DATEDIFF(DAY,IR.DOA,GETDATE()) as Days,P.Name as PatientName ,W.Name WardName,B.No as BedNo,R.RoomNo as RoomNo from IPDRecord IR join
Patient P on P.Id=IR.PatientId
join Bed B on B.Id=IR.BedId
join Room R on R.Id=IR.RoomId
join Ward W on W.Id=R.WardId
where 
(@BranchId is null Or IR.BranchId=@BranchId) AND
IR.DODC is null and IR.IsDelete=0
END
GO
