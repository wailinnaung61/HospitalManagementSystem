   
ALTER PROCEDURE [dbo].[SP_GetFoodCategories]     
        
    @FoodCategoryId INT = NULL,     
    @Name NVARCHAR(MAX) = NULL  
   
  
AS     
BEGIN     
    SET NOCOUNT ON     
   
    SELECT * FROM FoodCategory      
        WHERE      
             
            (@FoodCategoryId IS NULL OR Id=@FoodCategoryId) AND   
            (@Name IS NULL OR (Name LIKE '%' + @Name + '%' OR @Name LIKE '%' + Name + '%')) AND  
            IsDelete=0    
        ORDER BY Name     
END  