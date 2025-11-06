SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 02, 2022
-- Description: Get All PackingUnits
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetPackingUnits]
    @ItemId INT = NULL,
    @UnitId INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM PackingUnit 
        WHERE 
            ((@ItemId IS NOT NULL AND ItemId=@ItemId) OR 
            (@ItemId IS NULL AND ItemId IS NOT NULL)) AND
            ((@UnitId IS NOT NULL AND UnitId=@UnitId) OR 
            (@UnitId IS NULL AND UnitId IS NOT NULL))
        ORDER BY UnitLevel
END
GO
