SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 08, 2022
-- Description: Get All PurchaseItems
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetPurchaseItems]
    @PurchaseId INT = NULL,
    @ItemId INT = NULL,
    @UnitId INT = NULL,
    @BatchId INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM PurchaseItem 
        WHERE 
            ((@PurchaseId IS NOT NULL AND PurchaseId=@PurchaseId) OR 
            (@PurchaseId IS NULL AND PurchaseId IS NOT NULL)) AND
            ((@ItemId IS NOT NULL AND ItemId=@ItemId) OR 
            (@ItemId IS NULL AND ItemId IS NOT NULL)) AND
            ((@UnitId IS NOT NULL AND UnitId=@UnitId) OR 
            (@UnitId IS NULL AND UnitId IS NOT NULL)) AND
            ((@BatchId IS NOT NULL AND BatchId=@BatchId) OR 
            (@BatchId IS NULL AND BatchId IS NOT NULL))
        ORDER BY SortOrder
END
GO
