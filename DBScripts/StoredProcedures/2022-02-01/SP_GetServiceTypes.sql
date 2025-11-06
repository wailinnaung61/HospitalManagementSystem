 
Create PROCEDURE [dbo].[SP_GetServiceTypes]   
    @DepartmentId INT = NULL,   
    @ServiceTypeId INT = NULL   
AS   
BEGIN   
    SET NOCOUNT ON   
 
    SELECT * FROM ServiceType    
        WHERE    
            ((@DepartmentId IS NOT NULL AND DepartmentId=@DepartmentId) OR    
            (@DepartmentId IS NULL AND DepartmentId IS NOT NULL)) AND   
            ((@ServiceTypeId IS NOT NULL AND Id=@ServiceTypeId) OR    
            (@ServiceTypeId IS NULL AND Id IS NOT NULL))  and IsDelete=0  
        ORDER BY Name   
END 