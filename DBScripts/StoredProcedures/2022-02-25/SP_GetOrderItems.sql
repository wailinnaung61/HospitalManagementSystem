SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 25, 2022
-- Description: Get All OrderItems
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetOrderItems]
    @OrderId INT = NULL,
    @ItemId INT = NULL,
    @UnitId INT = NULL,
    @IsFOC BIT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT *, I.Name AS [ItemName], U.Name AS [UnitName] FROM OrderItem OI
    JOIN Item I ON OI.ItemId=I.Id
    JOIN Unit U ON OI.UnitId=U.Id
        WHERE 
            (@OrderId IS NOT NULL AND OrderId=@OrderId) AND
            (@ItemId IS NOT NULL AND ItemId=@ItemId) AND
            (@UnitId IS NOT NULL AND UnitId=@UnitId) AND
            (@IsFOC IS NOT NULL AND IsFOC=@IsFOC)
        ORDER BY SortOrder
END
GO
