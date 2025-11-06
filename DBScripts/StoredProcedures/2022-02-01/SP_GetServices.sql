 
CREATE PROCEDURE [dbo].[SP_GetServices]   
    @DepartmentId INT = NULL,   
    @ServiceTypeId INT = NULL,
    @ServiceId INT = NULL  
AS   
BEGIN   
    SET NOCOUNT ON   
 
    SELECT * FROM Service    
        WHERE    
            ((@DepartmentId IS NOT NULL AND DepartmentId=@DepartmentId) OR    
            (@DepartmentId IS NULL AND DepartmentId IS NOT NULL)) AND   
            ((@ServiceTypeId IS NOT NULL AND ServiceTypeId=@ServiceTypeId) OR    
            (@ServiceTypeId IS NULL AND ServiceTypeId IS NOT NULL)) AND
            ((@ServiceId IS NOT NULL AND Id=@ServiceId) OR    
            (@ServiceId IS NULL AND Id IS NOT NULL))
            and IsDelete=0  
        ORDER BY Name   
END 