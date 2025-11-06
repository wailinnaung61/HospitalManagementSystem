-- ============================================= 
-- Author:      Aung Naing OO 
-- Create Date: Jan 17, 2022 
-- Description: Get All Menus 
-- ============================================= 
Alter PROCEDURE [dbo].[SP_GetMenus] 
    @MenuId INT = NULL, 
    @IsActive INT = NULL, 
    @IsParent BIT = NULL, 
    @ParentId INT = NULL, 
    @IsGroup BIT = NULL,    
    @GroupId INT = NULL, 
    @CheckModule BIT = 1, 
    @ModuleId INT = NULL ,
    @IsBranchId BIT = NULL
    
AS 
BEGIN 
    -- SET NOCOUNT ON added to prevent extra result sets from 
    -- interfering with SELECT statements. 
    SET NOCOUNT ON 
 
    -- Insert statements for procedure here 
    -- 'Id IS NOT NULL' means to get all records 
    SELECT M.*, MD.Name AS [ModuleName] FROM [Menu] M 
        JOIN [Module] MD ON M.ModuleId = MD.Id 
        WHERE  
            ((@MenuId IS NOT NULL AND M.Id=@MenuId) OR  
            (@MenuId IS NULL AND M.Id IS NOT NULL)) AND 
             
            ((@IsActive IS NOT NULL AND M.IsActive=@IsActive) OR  
            (@IsActive IS NULL AND M.IsActive IS NOT NULL)) AND 
 
            ((@IsParent IS NOT NULL AND ( 
                (@IsParent = 1 AND ParentId IS NULL ) OR 
                (@IsParent = 0 AND ParentId IS NOT NULL) 
            )) OR  
            (@IsParent IS NULL AND M.Id IS NOT NULL)) AND 
 
            ((@ParentId IS NOT NULL AND ParentId=@ParentId) OR  
            (@ParentId IS NULL AND M.Id IS NOT NULL)) AND 
 
            ((@IsGroup IS NOT NULL AND IsGroup=@IsGroup) OR  
            (@IsGroup IS NULL AND M.Id IS NOT NULL)) AND 
 
            ((@GroupId IS NOT NULL AND GroupId=@GroupId) OR  
            (@GroupId IS NULL AND M.Id IS NOT NULL)) AND 
 
            ((@CheckModule = 1 AND MD.IsDelete = 0 AND MD.IsActive = 1) OR  
            (@CheckModule = 0 AND M.Id IS NOT NULL)) AND 
 
            ((@ModuleId IS NOT NULL AND ModuleId=@ModuleId) OR  
            (@ModuleId IS NULL AND M.Id IS NOT NULL)) AND 

            ((@IsBranchId IS NOT NULL AND M.IsBranchId IS NOT NULL) OR  
            (@IsBranchId IS NULL AND M.IsBranchId = 0)) AND 

            M.IsDelete = 0 
        ORDER BY MenuOrder 
END

