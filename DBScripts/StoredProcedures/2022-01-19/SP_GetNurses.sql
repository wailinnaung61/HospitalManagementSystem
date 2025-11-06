
CREATE PROCEDURE [dbo].[SP_GetNurses]  
    @DepartmentId INT = NULL,  
    @NurseId INT = NULL  
AS  
BEGIN  
    SET NOCOUNT ON  

    SELECT * FROM Nurse   
        WHERE   
            ((@DepartmentId IS NOT NULL AND DepartmentId=@DepartmentId) OR   
            (@DepartmentId IS NULL AND DepartmentId IS NOT NULL)) AND  
            ((@NurseId IS NOT NULL AND Id=@NurseId) OR   
            (@NurseId IS NULL AND Id IS NOT NULL))  and IsDelete=0 
        ORDER BY Name  
END 