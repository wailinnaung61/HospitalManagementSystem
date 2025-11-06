SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Jan 17, 2022
-- Description: Get All Menus
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetMenus]
    @MenuId INT = NULL,
    @IsActive INT = NULL,
    @IsParent INT = NULL,
    @ParentId INT = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    -- 'Id IS NOT NULL' means to get all records
    SELECT * FROM [Menu] 
        WHERE 
            ((@MenuId IS NOT NULL AND Id=@MenuId) OR 
            (@MenuId IS NULL AND Id IS NOT NULL)) AND
            
            ((@IsActive IS NOT NULL AND IsActive=@IsActive) OR 
            (@IsActive IS NULL AND IsActive IS NOT NULL)) AND

            ((@IsParent IS NOT NULL AND (
                (@IsParent = 1 AND ParentId IS NULL ) OR
                (@IsParent = 0 AND ParentId IS NOT NULL)
            )) OR 
            (@IsParent IS NULL AND Id IS NOT NULL)) AND

            ((@ParentId IS NOT NULL AND ParentId=@ParentId) OR 
            (@ParentId IS NULL AND Id IS NOT NULL)) AND

            IsDelete = 0
        ORDER BY MenuOrder
END
GO
