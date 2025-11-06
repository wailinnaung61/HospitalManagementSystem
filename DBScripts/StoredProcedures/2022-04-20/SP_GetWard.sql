SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_GetWard] 
    @FloorId int=NULL,
	@DepartmentId int=NULL,
	@WardId int=NULL,
    @Name NVARCHAR(MAX) = NULL 
AS
BEGIN
	SET NOCOUNT ON;

	Select W.*, D.Name AS [DepartmentName], F.Name AS [FloorName], O.Name AS [OutletName] from Ward W 
    JOIN Department D ON W.DepartmentId=D.Id
    JOIN [Floor] F ON W.FloorId=F.Id
    JOIN Outlet O ON W.OutletId=O.Id where             
    (@Name IS NULL OR (W.Name LIKE '%' + @Name + '%' OR @Name LIKE '%' + W.Name + '%')) AND
    (@FloorId is NULL or FloorId=@FloorId) AND
    (@DepartmentId is NULL or DepartmentId=@DepartmentId) AND
	(@WardId is NULL or W.Id=@WardId) AND
    W.IsDelete=0
END
GO
