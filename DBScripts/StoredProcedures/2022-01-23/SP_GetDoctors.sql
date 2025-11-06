SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[SP_GetDoctors]
    @DoctorId INT = NULL,
    @DepartmentId INT =NULL,
    @SpecialityId INT = NUll
AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM [Doctor] 
        WHERE 
            ((@DepartmentId IS NOT NULL AND DepartmentId=@DepartmentId) OR 
            (@DepartmentId IS NULL AND DepartmentId IS NOT NULL)) AND
             ((@SpecialityId IS NOT NULL AND SpecialityId=@SpecialityId) OR 
            (@SpecialityId IS NULL AND SpecialityId IS NOT NULL)) AND
             ((@DoctorId IS NOT NULL AND Id=@DoctorId) OR 
            (@DoctorId IS NULL AND Id IS NOT NULL)) AND
            IsDelete = 0
        ORDER BY Code
END
GO
