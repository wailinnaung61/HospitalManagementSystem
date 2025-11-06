SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
ALTER PROCEDURE [dbo].[SP_GetServices]
    @BranchId INT = NULL,
    @ServiceTypeId INT = NULL,
    @ServiceId INT = NULL  
AS   
BEGIN   
    SET NOCOUNT ON   
 
    SELECT * FROM Service    
        WHERE
            (@BranchId IS NULL OR BranchId=@BranchId) AND 
            (@ServiceTypeId IS NULL OR ServiceTypeId=@ServiceTypeId) AND
            (@ServiceId IS NULL OR Id=@ServiceId) AND
            IsDelete=0  
        ORDER BY Name   
END 
GO
