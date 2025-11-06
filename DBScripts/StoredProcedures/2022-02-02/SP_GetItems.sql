SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 02, 2022
-- Description: Get All Items
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetItems]
    @BranchId INT = NULL,
    @ItemId INT = NULL,
    @ItemTypeId INT = NULL,
    @Name NVARCHAR(MAX) = NULL,
    @Barcode NVARCHAR(MAX) = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM Item 
        WHERE 
            ((@BranchId IS NOT NULL AND BranchId=@BranchId) OR 
            (@BranchId IS NULL AND BranchId IS NOT NULL)) AND
            ((@ItemId IS NOT NULL AND Id=@ItemId) OR 
            (@ItemId IS NULL AND Id IS NOT NULL)) AND
            ((@ItemTypeId IS NOT NULL AND ItemTypeId=@ItemTypeId) OR 
            (@ItemTypeId IS NULL AND Id IS NOT NULL)) AND
            ((@Name IS NOT NULL AND (Name LIKE '%' + @Name + '%' OR @Name LIKE '%' + Name + '%')) OR 
            (@Name IS NULL AND Id IS NOT NULL)) AND
            ((@Barcode IS NOT NULL AND Barcode=@Barcode) OR 
            (@Barcode IS NULL AND Id IS NOT NULL)) AND
            IsDelete = 0
        ORDER BY Name
END
GO
