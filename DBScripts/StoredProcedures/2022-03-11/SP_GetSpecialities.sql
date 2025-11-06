SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_GetSpecialities]
    @BranchId INT = NULL,
    @SpecialityId INT = NULL
AS 
BEGIN

    SET NOCOUNT ON

    SELECT *
    FROM [Speciality]
    WHERE
        (@BranchId IS NULL OR BranchId=@BranchId) AND
        (@SpecialityId IS NULL OR Id=@SpecialityId) AND
        IsDelete = 0
END 
GO
