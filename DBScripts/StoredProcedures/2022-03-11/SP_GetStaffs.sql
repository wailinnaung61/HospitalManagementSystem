SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zun Pwint Phyu
-- Create date: 07_Feb_2022
-- Description:	Get Staffs
--=============================================
ALTER PROCEDURE [dbo].[SP_GetStaffs] 
	-- Add the parameters for the stored procedure here
    @BranchId int=null,
	@StaffId int=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Select * from Staff where IsDelete=0 and 
    (@BranchId is null or BranchId=@BranchId) and
    (@StaffId is null or Id=@StaffId)
END
GO
