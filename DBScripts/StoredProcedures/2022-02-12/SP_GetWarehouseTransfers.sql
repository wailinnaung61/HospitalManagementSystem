SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 12, 2022
-- Description: Get All WarehouseTransfers
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetWarehouseTransfers]
    @BranchId INT = NULL,
    @WarehouseTransferId INT = NULL,
    @FromWarehouseId INT = NULL,
    @ToWarehouseId INT = NULL,
    @ItemId INT = NULL,
    @Remark NVARCHAR(MAX) = NULL,
    @Date DATETIME2(7) = NULL,
    @StartDate DATETIME2(7) = NULL,
    @EndDate DATETIME2(7) = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT WT.*, FromW.Name AS [FromWarehouseName], ToW.Name AS [ToWarehouseName], B.Name AS [BranchName]
    FROM WarehouseTransfer WT
        JOIN Branch B ON WT.BranchId = B.Id
        JOIN Warehouse FromW ON WT.FromWarehouseId = FromW.Id
        JOIN Warehouse ToW ON WT.ToWarehouseId = FromW.Id
    WHERE 
        ((@BranchId IS NOT NULL AND WT.BranchId=@BranchId) OR
        (@BranchId IS NULL AND WT.Id IS NOT NULL)) AND
        ((@WarehouseTransferId IS NOT NULL AND WT.Id=@WarehouseTransferId) OR
        (@WarehouseTransferId IS NULL AND WT.Id IS NOT NULL)) AND
        ((@FromWarehouseId IS NOT NULL AND FromWarehouseId=@FromWarehouseId) OR
        (@FromWarehouseId IS NULL AND WT.Id IS NOT NULL)) AND
        ((@ToWarehouseId IS NOT NULL AND ToWarehouseId=@ToWarehouseId) OR
        (@ToWarehouseId IS NULL AND WT.Id IS NOT NULL)) AND
        ((@ItemId IS NOT NULL AND EXISTS 
            ( SELECT *
        FROM WarehouseTransferItem AS WI
        WHERE WI.WarehouseTransferId = WT.Id
            AND WI.ItemId=@ItemId
            )) OR
        (@ItemId IS NULL AND WT.Id IS NOT NULL)) AND
        ((@Remark IS NOT NULL AND (Remark LIKE '%' + @Remark + '%' OR @Remark LIKE '%' + Remark + '%')) OR
        (@Remark IS NULL AND WT.Id IS NOT NULL)) AND
        ((@Date IS NOT NULL AND [Date]=@Date) OR
        (@Date IS NULL AND WT.Id IS NOT NULL)) AND
        ((@StartDate IS NOT NULL AND [Date]>=@StartDate) OR
        (@StartDate IS NULL AND WT.Id IS NOT NULL)) AND
        ((@EndDate IS NOT NULL AND [Date]<=@EndDate) OR
        (@EndDate IS NULL AND WT.Id IS NOT NULL)) AND
        WT.IsDelete = 0
    ORDER BY [Date] DESC
END
GO
