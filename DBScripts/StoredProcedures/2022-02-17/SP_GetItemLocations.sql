SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_GetItemLocations]
    @BranchId INT = NULL,
    @ItemId INT = NULL,
    @LocationId INT = NULL,
    @ItemLocationId INT =NULL 

AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM ItemLocation 
        WHERE 
            ((@BranchId IS NOT NULL AND BranchId=@BranchId) OR 
            (@BranchId IS NULL AND BranchId IS NOT NULL)) AND
            ((@ItemId IS NOT NULL AND ItemId=@ItemId) OR 
            (@ItemId IS NULL AND ItemId IS NOT NULL)) AND
            ((@LocationId IS NOT NULL AND LocationId=@LocationId) OR 
            (@LocationId IS NULL AND LocationId IS NOT NULL)) AND
            ((@ItemLocationId IS NOT NULL AND Id=@ItemLocationId) OR 
            (@ItemLocationId IS NULL AND Id IS NOT NULL)) AND
            IsDelete = 0
END
GO
