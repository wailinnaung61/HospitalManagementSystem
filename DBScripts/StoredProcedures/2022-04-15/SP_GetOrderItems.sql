SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 25, 2022
-- Description: Get All OrderItems
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetOrderItems]
    @OrderId INT = NULL,
    @ItemId INT = NULL,
    @UnitId INT = NULL,
    @IsFOC BIT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT OI.*, I.Name AS [ItemName], U.Name AS [UnitName], U.ShortForm AS [ShortForm] FROM OrderItem OI
    JOIN Item I ON OI.ItemId=I.Id
    JOIN Unit U ON OI.UnitId=U.Id
        WHERE 
            (@OrderId IS NULL OR OrderId=@OrderId) AND
            (@ItemId IS NULL OR ItemId=@ItemId) AND
            (@UnitId IS NULL OR UnitId=@UnitId) AND
            (@IsFOC IS NULL OR IsFOC=@IsFOC)
        ORDER BY SortOrder
END
GO
