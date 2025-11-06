USE [hms_db]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetBranches]    Script Date: 3/17/2022 1:32:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Jan 13, 2022
-- Description: Get All Branches
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetBranches]
    @BranchId INT = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT B.*,null as StateName,null as CityName,null as TownshipName FROM Branch B
	join State S on B.StateId=S.Id
	join City C on B.CityId=C.Id
	join Township T on B.TownshipId=T.Id
    WHERE 
            (@BranchId IS NULL OR B.Id=@BranchId) AND
            B.IsDelete = 0
        ORDER BY B.Name
END