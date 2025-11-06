SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 14, 2022
-- Description: Get All WarehouseTransferItems
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetWarehouseTransferItems]
    @WarehouseTransferId INT = NULL,
    @ItemId INT = NULL,
    @UnitId INT = NULL,
    @BatchId INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM WarehouseTransferItem 
        WHERE 
            ((@WarehouseTransferId IS NOT NULL AND WarehouseTransferId=@WarehouseTransferId) OR 
            (@WarehouseTransferId IS NULL AND WarehouseTransferId IS NOT NULL)) AND
            ((@ItemId IS NOT NULL AND ItemId=@ItemId) OR 
            (@ItemId IS NULL AND ItemId IS NOT NULL)) AND
            ((@UnitId IS NOT NULL AND UnitId=@UnitId) OR 
            (@UnitId IS NULL AND UnitId IS NOT NULL)) AND
            ((@BatchId IS NOT NULL AND BatchId=@BatchId) OR 
            (@BatchId IS NULL AND BatchId IS NOT NULL))
        ORDER BY SortOrder
END
GO
