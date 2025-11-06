
CREATE PROCEDURE [dbo].[SP_GetItemsForExpirationRemind] 
    @BranchId INT = NULL, 
    @ItemId INT = NULL, 
    @ItemTypeId INT = NULL, 
    @Name NVARCHAR(MAX) = NULL, 
    @Code NVARCHAR(MAX) = NULL, 
    @Barcode NVARCHAR(MAX) = NULL 
AS 
BEGIN 
 
    SET NOCOUNT ON 
 
    select * from Item I
    join Batch B on B.ItemId=I.Id 
        WHERE  
            (@BranchId IS NULL OR I.BranchId=@BranchId) AND 
            (@ItemId IS NULL OR I.Id=@ItemId) AND 
            (@ItemTypeId IS NULL OR ItemTypeId=@ItemTypeId) AND 
            (@Name IS NULL OR (I.Name LIKE '%' + @Name + '%' OR @Name LIKE '%' + I.Name + '%')) AND 
            (@Code IS NULL OR (I.Code LIKE '%' + @Code + '%' OR @Code LIKE '%' + I.Code + '%')) AND 
            (@Barcode IS NULL OR Barcode=@Barcode) AND 
            (DATEDIFF(Day,GETDATE(),B.ExpiryDate)<I.ExpirationRemindDay and I.ExpirationRemindDay is not null) AND
            I.IsDelete = 0 
        ORDER BY I.Name 
END

