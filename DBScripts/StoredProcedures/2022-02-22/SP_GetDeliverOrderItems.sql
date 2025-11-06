
CREATE PROCEDURE [dbo].[SP_GetDeliverOrderItems] 
    @DeliverOrderId INT = NULL, 
    @ItemId INT = NULL, 
    @UnitId INT = NULL, 
    @BatchId INT = NULL 
AS 
BEGIN 
 
    SET NOCOUNT ON 
 
    SELECT * FROM DeliverOrderItem  
        WHERE  
            ((@DeliverOrderId IS NOT NULL AND DeliverOrderId=@DeliverOrderId) OR  
            (@DeliverOrderId IS NULL AND DeliverOrderId IS NOT NULL)) AND 
            ((@ItemId IS NOT NULL AND ItemId=@ItemId) OR  
            (@ItemId IS NULL AND ItemId IS NOT NULL)) AND 
            ((@UnitId IS NOT NULL AND UnitId=@UnitId) OR  
            (@UnitId IS NULL AND UnitId IS NOT NULL)) AND 
            ((@BatchId IS NOT NULL AND BatchId=@BatchId) OR  
            (@BatchId IS NULL AND BatchId IS NOT NULL)) 
        ORDER BY SortOrder 
END

