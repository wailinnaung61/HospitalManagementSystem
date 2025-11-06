SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 08, 2022
-- Description: Get All Purchases
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetPurchases]
    @BranchId INT = NULL,
    @PurchaseId INT = NULL,
    @WarehouseId INT = NULL,
    @ItemId INT = NULL,
    @VoucherNo NVARCHAR(MAX) = NULL,
    @Supplier NVARCHAR(MAX) = NULL,
    @PurchaseDate DATETIME2(7) = NULL,
    @StartPurchaseDate DATETIME2(7) = NULL,
    @EndPurchaseDate DATETIME2(7) = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT P.* FROM Purchase P
        WHERE 
            ((@BranchId IS NOT NULL AND BranchId=@BranchId) OR 
            (@BranchId IS NULL AND Id IS NOT NULL)) AND
            ((@PurchaseId IS NOT NULL AND Id=@PurchaseId) OR 
            (@PurchaseId IS NULL AND Id IS NOT NULL)) AND
            ((@WarehouseId IS NOT NULL AND WarehouseId=@WarehouseId) OR 
            (@WarehouseId IS NULL AND Id IS NOT NULL)) AND
            ((@ItemId IS NOT NULL AND EXISTS 
            ( SELECT *
                FROM PurchaseItem AS PI
                WHERE PI.PurchaseId = P.Id
                AND PI.ItemId=@ItemId
            )) OR 
            (@ItemId IS NULL AND Id IS NOT NULL)) AND
            ((@VoucherNo IS NOT NULL AND (VoucherNo LIKE '%' + @VoucherNo + '%' OR @VoucherNo LIKE '%' + VoucherNo + '%')) OR 
            (@VoucherNo IS NULL AND Id IS NOT NULL)) AND
            ((@Supplier IS NOT NULL AND (Supplier LIKE '%' + @Supplier + '%' OR @Supplier LIKE '%' + Supplier + '%')) OR 
            (@Supplier IS NULL AND Id IS NOT NULL)) AND
            ((@PurchaseDate IS NOT NULL AND PurchaseDate=@PurchaseDate) OR 
            (@PurchaseDate IS NULL AND Id IS NOT NULL)) AND
            ((@StartPurchaseDate IS NOT NULL AND PurchaseDate>=@StartPurchaseDate) OR 
            (@StartPurchaseDate IS NULL AND Id IS NOT NULL)) AND
            ((@EndPurchaseDate IS NOT NULL AND PurchaseDate<=@EndPurchaseDate) OR 
            (@EndPurchaseDate IS NULL AND Id IS NOT NULL)) AND
            IsDelete = 0
        ORDER BY [VoucherNo] DESC
END
GO
