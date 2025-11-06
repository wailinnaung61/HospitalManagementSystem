SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[SP_GetWareHouses]
    @BranchId INT = NULL,
    @WareHouseId INT = NULL

AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM WareHouse 
        WHERE 
            (@BranchId IS NOT NULL AND BranchId=@BranchId) OR 
            (@BranchId IS NULL AND BranchId IS NOT NULL) AND
            (@WareHouseId IS NOT NULL AND Id=@WareHouseId) OR 
            (@WareHouseId IS NULL AND Id IS NOT NULL) AND
            IsDelete = 0
        ORDER BY Name
END
GO

