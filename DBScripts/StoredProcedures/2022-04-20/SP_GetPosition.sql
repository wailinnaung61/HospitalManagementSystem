USE [msis_hms_db]
GO
/****** Object:  StoredProcedure [dbo].[SP_GetPosition]    Script Date: 4/20/2022 10:54:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
ALTER PROCEDURE [dbo].[SP_GetPosition]   
      
    @PositionId INT = NULL,   
    @Name NVARCHAR(MAX) = NULL,
	@Code NVARCHAR(MAX) = NULL

AS   
BEGIN   
    SET NOCOUNT ON   
 
    SELECT * FROM Position    
        WHERE    
           
            (@PositionId IS NULL OR Id=@PositionId) AND 
            (@Name IS NULL OR (Name LIKE '%' + @Name + '%' OR @Name LIKE '%' + Name + '%')) AND
			(@Code IS NULL OR Code=@Code) AND
            IsDelete=0  
        ORDER BY Name   
END
