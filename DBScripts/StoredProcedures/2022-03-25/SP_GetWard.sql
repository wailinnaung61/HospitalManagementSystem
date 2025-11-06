SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[SP_GetWard] 
    @FloorId int=NULL,
	@DepartmentId int=NULL,
	@WardId int=NULL,
    @Name NVARCHAR(MAX) = NULL 
AS
BEGIN
	SET NOCOUNT ON;

	Select * from Ward where             
    (@Name IS NULL OR (Name LIKE '%' + @Name + '%' OR @Name LIKE '%' + Name + '%')) AND
    (@FloorId is NULL or FloorId=@FloorId) AND
    (@DepartmentId is NULL or DepartmentId=@DepartmentId) AND
	(@WardId is NULL or Id=@WardId) AND
    IsDelete=0
END
GO
