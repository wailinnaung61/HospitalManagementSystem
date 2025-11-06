GO
/****** Object:  StoredProcedure [dbo].[SP_GetFoods]    Script Date: 4/21/2022 1:50:35 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  
CREATE PROCEDURE [dbo].[SP_GetAllotments]   

@IPDRecordId int = NULL,
@IPDAllotmentId int = NULL,
@IPDOrderId int = NULL,
@IPDFromBedId int = NULL,
@IPDToBedId int = NULL,
@IPDFromRoomId int = NULL,
@IPDToRoomId int = NULL,
@Reason NVARCHAR(MAX) = NULL

AS  
BEGIN  
  
 SET NOCOUNT ON;  
  
 Select a.*,r.RoomNo as [FromRoomNo],b.No as [FromBedNo],r1.RoomNo as [ToRoomNo],b1.No as [ToBedNo] from IPDAllotment a
	 Join Bed b On b.Id=a.FromBedId
	 Join Room r On r.Id=a.FromRoomId
	 left Join Bed b1 On b.Id=a.ToBedId
	 left Join Room r1 On r.Id=a.ToRoomId
	 where  
    (@IPDRecordId is NULL OR a.IPDRecordId=@IPDRecordId) AND  
    (@IPDAllotmentId is NULL OR a.Id=@IPDAllotmentId) AND
	(@IPDOrderId is NULL OR a.IPDOrderId=@IPDOrderId)AND
	(@IPDFromBedId is NULL OR a.FromBedId=@IPDFromBedId)AND
	(@IPDToBedId is NULL OR a.ToBedId=@IPDToBedId)AND
	(@IPDFromRoomId is NULL OR a.FromRoomId=@IPDFromRoomId)AND
	(@IPDToRoomId is NULL OR a.ToRoomId=@IPDToRoomId)AND
	(@Reason IS NULL OR (a.Reason LIKE '%' + @Reason + '%' OR @Reason LIKE '%' + a.Reason + '%')) AND
     a.IsDelete=0          
END  

