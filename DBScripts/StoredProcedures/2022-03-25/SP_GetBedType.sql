SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
Create PROCEDURE [dbo].[SP_GetBedType]   
   
    @BedTypeId INT = NULL,  
    @Name NVARCHAR(MAX) = NULL 
AS   
BEGIN   
    SET NOCOUNT ON   
 
    SELECT * FROM BedType    
        WHERE    
            
            (@BedTypeId IS NULL OR Id=@BedTypeId) AND 
            (@Name IS NULL OR (Name LIKE '%' + @Name + '%' OR @Name LIKE '%' + Name + '%')) AND
            IsDelete=0  
        ORDER BY Name   
END
GO
