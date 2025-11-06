SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create PROCEDURE [dbo].[SP_GetBed] 
    @RoomId int=NULL,
	@BedTypeId int=NULL,
	@BedId int=NULL
AS
BEGIN

	SET NOCOUNT ON;

	Select * from Bed where
    (@RoomId is NULL or RoomId=@RoomId) AND
	(@BedTypeId is NULL or BedTypeId=@BedTypeId) AND
    (@BedId is NULL or Id=@BedId) AND IsDelete=0  
END
GO