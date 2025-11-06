SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 02, 2022
-- Description: Get All Items
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetItems]
    @BranchId INT = NULL,
    @ItemId INT = NULL,
    @ItemTypeId INT = NULL,
    @Name NVARCHAR(MAX) = NULL,
    @Code NVARCHAR(MAX) = NULL,
    @Barcode NVARCHAR(MAX) = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM Item 
        WHERE 
            (@BranchId IS NULL OR BranchId=@BranchId) AND
            (@ItemId IS NULL OR Id=@ItemId) AND
            (@ItemTypeId IS NULL OR ItemTypeId=@ItemTypeId) AND
            (@Name IS NULL OR (Name LIKE '%' + @Name + '%' OR @Name LIKE '%' + Name + '%')) AND
            (@Code IS NULL OR (Code LIKE '%' + @Code + '%' OR @Code LIKE '%' + Code + '%')) AND
            (@Barcode IS NULL OR Barcode=@Barcode) AND
            IsDelete = 0
        ORDER BY Name
END
GO
