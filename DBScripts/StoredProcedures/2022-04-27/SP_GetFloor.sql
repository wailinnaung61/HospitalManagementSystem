ALTER PROCEDURE [dbo].[SP_GetFloor]   
    @BranchId int=NULL,  
 @FloorId int=NULL,  
    @Name NVARCHAR(MAX) = NULL  
AS  
BEGIN  
  
 SET NOCOUNT ON;  
  
 Select * from Floor where IsDelete=0 AND   
    (@BranchId is null or BranchId=@BranchId) AND  
    (@FloorId is null or Id=@FloorId) AND  
    (@Name IS NULL OR (Name LIKE '%' + @Name + '%' OR @Name LIKE '%' + Name + '%'))   
  
END  