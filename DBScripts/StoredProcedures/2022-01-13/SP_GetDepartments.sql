SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Jan 13, 2022
-- Description: Get All Departments By Branch
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetDepartments]
    @BranchId INT = NULL,
    @DepartmentId INT = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT * FROM Department 
        WHERE 
            ((@BranchId IS NOT NULL AND BranchId=@BranchId) OR 
            (@BranchId IS NULL AND BranchId IS NOT NULL)) AND
            ((@DepartmentId IS NOT NULL AND Id=@DepartmentId) OR 
            (@DepartmentId IS NULL AND Id IS NOT NULL)) AND
            IsDelete = 0
        ORDER BY Name
END
GO
