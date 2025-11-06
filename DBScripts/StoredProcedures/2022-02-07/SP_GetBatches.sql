SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 05, 2022
-- Description: Get All Batches
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetBatches]
    @BranchId INT = NULL,
    @ItemId INT = NULL,
    @BatchId INT = NULL,
    @BatchName NVARCHAR(MAX) = NULL,
    @BatchCode NVARCHAR(MAX) = NULL,
    @BatchNumber NVARCHAR(MAX) = NULL,
    @ExpiryDate DATETIME2(7) = NULL,
    @StartExpiryDate DATETIME2(7) = NULL,
    @EndExpiryDate DATETIME2(7) = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM Batch 
        WHERE 
            ((@BranchId IS NOT NULL AND BranchId=@BranchId) OR 
            (@BranchId IS NULL AND Id IS NOT NULL)) AND
            ((@ItemId IS NOT NULL AND ItemId=@ItemId) OR 
            (@ItemId IS NULL AND Id IS NOT NULL)) AND
            ((@BatchId IS NOT NULL AND Id=@BatchId) OR 
            (@BatchId IS NULL AND Id IS NOT NULL)) AND
            ((@BatchName IS NOT NULL AND (Name LIKE '%' + @BatchName + '%' OR @BatchName LIKE '%' + Name + '%')) OR 
            (@BatchName IS NULL AND Id IS NOT NULL)) AND
            ((@BatchCode IS NOT NULL AND (Code LIKE '%' + @BatchCode + '%' OR @BatchCode LIKE '%' + Code + '%')) OR 
            (@BatchCode IS NULL AND Id IS NOT NULL)) AND
            ((@BatchNumber IS NOT NULL AND (BatchNumber LIKE '%' + @BatchNumber + '%' OR @BatchNumber LIKE '%' + BatchNumber + '%')) OR 
            (@BatchNumber IS NULL AND Id IS NOT NULL)) AND
            ((@ExpiryDate IS NOT NULL AND ExpiryDate=@ExpiryDate) OR 
            (@ExpiryDate IS NULL AND Id IS NOT NULL)) AND
            ((@StartExpiryDate IS NOT NULL AND ExpiryDate>=@StartExpiryDate) OR 
            (@StartExpiryDate IS NULL AND Id IS NOT NULL)) AND
            ((@EndExpiryDate IS NOT NULL AND ExpiryDate<=@EndExpiryDate) OR 
            (@EndExpiryDate IS NULL AND Id IS NOT NULL)) AND
            IsDelete = 0
        ORDER BY [Name]
END
GO
