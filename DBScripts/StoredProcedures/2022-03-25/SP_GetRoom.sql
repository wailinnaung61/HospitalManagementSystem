SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetRoom] 
    @WardId int=NULL,
	@RoomTypeId int=NULL,
	@RoomId int=NULL,
    @RoomNo NVARCHAR(MAX) = NULL 
AS
BEGIN	
	SET NOCOUNT ON;

	Select * from Room WHERE 
    (@WardId is NULL or WardId=@WardId) AND
	(@RoomTypeId is NULL or RoomTypeId=@RoomTypeId) AND
    (@RoomId is NULL or Id=@RoomId) AND
    (@RoomNo is NULL or RoomNo=@RoomNo) AND
    IsDelete=0  

END
GO
