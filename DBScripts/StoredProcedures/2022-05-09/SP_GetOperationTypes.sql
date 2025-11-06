
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetOperationTypes]
    @BranchId INT = NULL,
    @OperationTypeId INT = NULL

AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM OperationType 
        WHERE 
            (@BranchId IS NULL OR BranchId=@BranchId)AND
            (@OperationTypeId IS NULL OR Id=@OperationTypeId) AND
            IsDelete = 0
        ORDER BY [Name]
END