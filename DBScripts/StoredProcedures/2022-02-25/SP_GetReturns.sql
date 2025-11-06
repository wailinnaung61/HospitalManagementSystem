USE [hms_db]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetOutletTransfers]    Script Date: 2/25/2022 10:19:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Zun Pwint Phyu
-- Create Date: Feb 25, 2022
-- Description: Get All GetReturns
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetReturns]
    @BranchId INT = NULL,
    @ReturnId INT = NULL,
    @ToWarehouseId INT = NULL,
    @FromOutletId INT = NULL,
    @ItemId INT = NULL,
    @Remark NVARCHAR(MAX) = NULL,
    @Date DATETIME2(7) = NULL,
    @StartDate DATETIME2(7) = NULL,
    @EndDate DATETIME2(7) = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT R.*, FromW.Name AS [WarehouseName], ToOut.Name AS [OutletName], B.Name AS [BranchName]
    FROM [Return] R
        JOIN Branch B ON R.BranchId = B.Id
        JOIN Warehouse FromW ON R.WarehouseId = FromW.Id
        JOIN Outlet ToOut ON R.OutletId = ToOut.Id
    WHERE 
        ((@BranchId IS NOT NULL AND R.BranchId=@BranchId) OR
        (@BranchId IS NULL AND R.Id IS NOT NULL)) AND
        ((@ReturnId IS NOT NULL AND R.Id=@ReturnId) OR
        (@ReturnId IS NULL AND R.Id IS NOT NULL)) AND
        ((@ToWarehouseId IS NOT NULL AND R.WarehouseId=@ToWarehouseId) OR
        (@ToWarehouseId IS NULL AND R.Id IS NOT NULL)) AND
        ((@FromOutletId IS NOT NULL AND R.OutletId=@FromOutletId) OR
        (@FromOutletId IS NULL AND R.Id IS NOT NULL)) AND
        ((@ItemId IS NOT NULL AND EXISTS 
            ( SELECT *
        FROM OutletTransferItem AS WI
        WHERE WI.OutletTransferId = R.Id
            AND WI.ItemId=@ItemId
            )) OR
        (@ItemId IS NULL AND R.Id IS NOT NULL)) AND
        ((@Remark IS NOT NULL AND (Remark LIKE '%' + @Remark + '%' OR @Remark LIKE '%' + Remark + '%')) OR
        (@Remark IS NULL AND R.Id IS NOT NULL)) AND
        ((@Date IS NOT NULL AND [Date]=@Date) OR
        (@Date IS NULL AND R.Id IS NOT NULL)) AND
        ((@StartDate IS NOT NULL AND [Date]>=@StartDate) OR
        (@StartDate IS NULL AND R.Id IS NOT NULL)) AND
        ((@EndDate IS NOT NULL AND [Date]<=@EndDate) OR
        (@EndDate IS NULL AND R.Id IS NOT NULL)) AND
        R.IsDelete = 0
    ORDER BY [Date] DESC
END