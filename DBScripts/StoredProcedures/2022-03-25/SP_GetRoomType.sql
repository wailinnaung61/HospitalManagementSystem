SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE PROCEDURE [dbo].[SP_GetRoomType]   
      
    @RoomTypeId INT = NULL,   
    @Name NVARCHAR(MAX) = NULL 

AS   
BEGIN   
    SET NOCOUNT ON   
 
    SELECT * FROM RoomType    
        WHERE    
           
            (@RoomTypeId IS NULL OR Id=@RoomTypeId) AND 
            (@Name IS NULL OR (Name LIKE '%' + @Name + '%' OR @Name LIKE '%' + Name + '%')) AND
            IsDelete=0  
        ORDER BY Name   
END
GO

