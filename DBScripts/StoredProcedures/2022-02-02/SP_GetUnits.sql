SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 02, 2022
-- Description: Get All Units
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetUnits]
    @BranchId INT = NULL,
    @UnitId INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM Unit 
        WHERE 
            ((@BranchId IS NOT NULL AND BranchId=@BranchId) OR 
            (@BranchId IS NULL AND Id IS NOT NULL)) AND
            ((@UnitId IS NOT NULL AND Id=@UnitId) OR 
            (@UnitId IS NULL AND Id IS NOT NULL)) AND
            IsDelete = 0
        ORDER BY UnitLevel, Name
END
GO
