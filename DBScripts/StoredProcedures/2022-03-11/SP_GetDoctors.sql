SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_GetDoctors]
    @BranchId INT = NULL,
    @DoctorId INT = NULL,
    @DepartmentId INT =NULL,
    @SpecialityId INT = NUll
AS
BEGIN

    SET NOCOUNT ON

    SELECT *
    FROM [Doctor]
    WHERE
        (@BranchId IS NULL OR BranchId=@BranchId) AND
        (@DoctorId IS NULL OR Id=@DoctorId) AND
        (@DepartmentId IS NULL OR DepartmentId=@DepartmentId) AND
        (@SpecialityId IS NULL OR SpecialityId=@SpecialityId) AND
        IsDelete = 0
    ORDER BY Code
END
GO
