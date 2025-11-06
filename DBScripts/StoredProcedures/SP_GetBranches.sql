SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Jan 13, 2022
-- Description: Get All Branches
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetBranches]
    @BranchId INT = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT * FROM Branch 
        WHERE 
            (@BranchId IS NOT NULL AND Id=@BranchId) OR 
            (@BranchId IS NULL AND Id IS NOT NULL) AND
            IsDelete = 0
        ORDER BY Name
END
GO
