/****** Object:  StoredProcedure [dbo].[SP_GetOrderItems]    Script Date: 5/13/2022 9:13:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Zun Pwint Phyu
-- Create Date: May 13, 2022
-- Description: Get OperationItem by OperationTreaterId
-- =============================================
Create PROCEDURE [dbo].[SP_GetOperationItems]
    @OperationTreaterId INT = NULL,
    @ItemId INT = NULL,
    @UnitId INT = NULL,
    @IsFOC BIT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT OI.*, I.Name AS [ItemName], U.Name AS [UnitName],U.ShortForm as ShortForm FROM OperationItem OI
    JOIN Item I ON OI.ItemId=I.Id
    JOIN Unit U ON OI.UnitId=U.Id
        WHERE 
            (@OperationTreaterId IS NULL OR OperationTreaterId=@OperationTreaterId) AND
            (@ItemId IS NULL OR ItemId=@ItemId) AND
            (@UnitId IS NULL OR UnitId=@UnitId) AND
            (@IsFOC IS NULL OR IsFOC=@IsFOC)
        ORDER BY SortOrder
END