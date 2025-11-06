SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_GetNurses]
    @BranchId INT = NULL,
    @DepartmentId INT = NULL,
    @NurseId INT = NULL
AS  
BEGIN
    SET NOCOUNT ON

    SELECT *
    FROM Nurse
    WHERE   
        (@BranchId IS NULL OR BranchId=@BranchId) AND
        (@DepartmentId IS NULL OR DepartmentId=@DepartmentId) AND
        (@NurseId IS NULL OR Id=@NurseId) and
        IsDelete=0
    ORDER BY Name
END 
GO
