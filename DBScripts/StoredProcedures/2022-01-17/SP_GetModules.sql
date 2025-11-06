SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Jan 17, 2022
-- Description: Get All Modules
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetModules]
    @ModuleId INT = NULL
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
    SELECT * FROM [Module] 
        WHERE 
            ((@ModuleId IS NOT NULL AND Id=@ModuleId) OR 
            (@ModuleId IS NULL AND Id IS NOT NULL)) AND
            IsDelete = 0
        ORDER BY ModuleOrder
END
GO
