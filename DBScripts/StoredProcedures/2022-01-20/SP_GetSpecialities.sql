
CREATE PROCEDURE [dbo].[SP_GetSpecialities] 
    @SpecialityId INT = NULL 
AS 
BEGIN 
 
    SET NOCOUNT ON 
 
    SELECT * FROM [Speciality]  
        WHERE  
            ((@SpecialityId IS NOT NULL AND Id=@SpecialityId) OR  
            (@SpecialityId IS NULL AND Id IS NOT NULL)) AND 
            IsDelete = 0         
END 