SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetVisitTypes]
    @BranchId INT = NULL,
    @VisitTypeId INT = NULL

AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM VisitType 
        WHERE 
            (@BranchId IS NULL OR BranchId=@BranchId)AND
            (@VisitTypeId IS NULL OR Id=@VisitTypeId) AND
            IsDelete = 0
        ORDER BY [Type]
END
GO
