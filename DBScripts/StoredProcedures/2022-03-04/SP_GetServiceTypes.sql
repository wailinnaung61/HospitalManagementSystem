SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
ALTER PROCEDURE [dbo].[SP_GetServiceTypes]   
    @BranchId INT = NULL,   
    @ServiceTypeId INT = NULL   
AS   
BEGIN   
    SET NOCOUNT ON   
 
    SELECT * FROM ServiceType    
        WHERE    
            (@BranchId IS NULL OR BranchId=@BranchId) AND 
            (@ServiceTypeId IS NULL OR Id=@ServiceTypeId) AND 
            IsDelete=0  
        ORDER BY Name   
END 
GO
