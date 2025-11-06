SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetItemTypes]
    @BranchId INT = NULL,
    @ItemTypeId INT = NULL

AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM ItemType 
        WHERE 
            (@BranchId IS NOT NULL AND BranchId=@BranchId) OR 
            (@BranchId IS NULL AND BranchId IS NOT NULL) AND
            (@ItemTypeId IS NOT NULL AND Id=@ItemTypeId) OR 
            (@ItemTypeId IS NULL AND Id IS NOT NULL) AND
            IsDelete = 0
        ORDER BY Name
END
GO

