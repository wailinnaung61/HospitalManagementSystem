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
-- Create date: 2022,June 07
-- Description:	<Description,,>
-- =============================================
Alter PROCEDURE SP_GetCFFee
	-- Add the parameters for the stored procedure here
	@BranchId int=null,
	@Status int =null,
	@StartDate datetime2(7)=null,
	@EndDate datetime2(7)=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
select sum(D.CFFEE) from Visit V join Doctor D on D.Id=V.DoctorId
where 
(@BranchId IS NULL OR V.BranchId=@BranchId) AND
(@Status IS NULL OR V.Status=@Status) And
(@StartDate Is NULL OR Convert(date,V.Date)>=Convert(date,@StartDate))And
(@EndDate Is NULL OR Convert(date,V.Date)<=Convert(date,@EndDate))And
V.IsDelete=0
END
GO
