SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 11, 2022
-- Description: Get All WarehouseItems
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetWarehouseItems]
    @BranchId INT = NULL,
    @WarehouseId INT = NULL,
    @ItemId INT = NULL,
    @BatchId INT = NULL,
    @ExpiryDate DATETIME2(7) = NULL,
    @StartExpiryDate DATETIME2(7) = NULL,
    @EndExpiryDate DATETIME2(7) = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT WI.*, W.Name AS [WarehouseName], I.Name AS [ItemName], I.Code AS [ItemCode], BT.Name AS [BatchName], BT.ExpiryDate AS [ExpiryDate], W.BranchId, B.Name AS [BranchName] FROM WarehouseItem WI
        JOIN Warehouse W ON WI.WarehouseId = W.Id
        JOIN Branch B ON W.BranchId = B.Id
        JOIN Item I ON WI.ItemId = I.Id
        JOIN Batch BT ON WI.BatchId = BT.Id
        WHERE 
            ((@BranchId IS NOT NULL AND W.BranchId=@BranchId) OR 
            (@BranchId IS NULL AND WarehouseId IS NOT NULL)) AND
            ((@WarehouseId IS NOT NULL AND WarehouseId=@WarehouseId) OR 
            (@WarehouseId IS NULL AND WarehouseId IS NOT NULL)) AND
            ((@ItemId IS NOT NULL AND WI.ItemId=@ItemId) OR 
            (@ItemId IS NULL AND WI.ItemId IS NOT NULL)) AND
            ((@BatchId IS NOT NULL AND BatchId=@BatchId) OR 
            (@BatchId IS NULL AND BatchId IS NOT NULL)) AND
            ((@ExpiryDate IS NOT NULL AND ExpiryDate=@ExpiryDate) OR 
            (@ExpiryDate IS NULL AND WarehouseId IS NOT NULL)) AND
            ((@StartExpiryDate IS NOT NULL AND ExpiryDate>=@StartExpiryDate) OR 
            (@StartExpiryDate IS NULL AND WarehouseId IS NOT NULL)) AND
            ((@EndExpiryDate IS NOT NULL AND ExpiryDate<=@EndExpiryDate) OR 
            (@EndExpiryDate IS NULL AND WarehouseId IS NOT NULL)) AND
            W.IsDelete = 0
        ORDER BY B.Code, W.Code, I.Name, BT.ExpiryDate
END
GO
