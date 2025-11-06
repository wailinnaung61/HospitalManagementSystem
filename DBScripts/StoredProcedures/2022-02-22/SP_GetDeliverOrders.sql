CREATE PROCEDURE [dbo].[SP_GetDeliverOrders] 
    @BranchId INT = NULL, ---
    @DeliverOrderId INT = NULL, ---
    @WarehouseId INT = NULL, ---
    @ItemId INT = NULL, ---
    @VoucherNo NVARCHAR(MAX) = NULL, ---
    @Customer NVARCHAR(MAX) = NULL, ---
    @Date DATETIME2(7) = NULL, 
    @StartDate DATETIME2(7) = NULL, 
    @EndDate DATETIME2(7) = NULL 
AS 
BEGIN 
 
    SET NOCOUNT ON 
 
    SELECT D.* FROM DeliverOrder D
        WHERE  
            ((@BranchId IS NOT NULL AND BranchId=@BranchId) OR  
            (@BranchId IS NULL AND Id IS NOT NULL)) AND 

            ((@DeliverOrderId IS NOT NULL AND Id=@DeliverOrderId) OR  
            (@DeliverOrderId IS NULL AND Id IS NOT NULL)) AND 

            ((@WarehouseId IS NOT NULL AND WarehouseId=@WarehouseId) OR  
            (@WarehouseId IS NULL AND Id IS NOT NULL)) AND 
            ((@ItemId IS NOT NULL AND EXISTS  
            ( SELECT * 
                FROM DeliverOrderItem AS PI 
                WHERE PI.DeliverOrderId = D.Id 
                AND PI.ItemId=@ItemId 
            )) OR  
            (@ItemId IS NULL AND Id IS NOT NULL)) AND

            ((@VoucherNo IS NOT NULL AND (VoucherNo LIKE '%' + @VoucherNo + '%' OR @VoucherNo LIKE '%' + VoucherNo + '%')) OR  
            (@VoucherNo IS NULL AND Id IS NOT NULL)) AND

            ((@Customer IS NOT NULL AND (Customer LIKE '%' + @Customer + '%' OR @Customer LIKE '%' + Customer + '%')) OR  
            (@Customer IS NULL AND Id IS NOT NULL)) AND 

            ((@Date IS NOT NULL AND D.Date=@Date) OR  
            (@Date IS NULL AND Id IS NOT NULL)) AND 
            ((@StartDate IS NOT NULL AND D.Date>=@StartDate) OR  
            (@StartDate IS NULL AND Id IS NOT NULL)) AND 
            ((@EndDate IS NOT NULL AND D.Date<=@EndDate) OR  
            (@EndDate IS NULL AND Id IS NOT NULL)) AND 

            IsDelete = 0 
        ORDER BY [VoucherNo] DESC 
END
