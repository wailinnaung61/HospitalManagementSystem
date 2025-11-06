        
Alter PROCEDURE [dbo].[SP_GetAllotments]         
      
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
  left join Bed b On b.Id=a.FromBedId      
  left Join Room r On r.Id=a.FromRoomId      
  left join Bed b1 On b1.Id=a.ToBedId      
  Join Room r1 On r1.Id=a.ToRoomId  
    
  where        
 (@IPDRecordId is NULL OR a.IPDRecordId=@IPDRecordId) AND        
 (@IPDAllotmentId is NULL OR a.Id=@IPDAllotmentId) AND      
 (@IPDOrderId is NULL OR a.IPDOrderId=@IPDOrderId)AND      
 (@IPDFromBedId is NULL OR a.FromBedId=@IPDFromBedId)AND      
 (@IPDToBedId is NULL OR a.ToBedId=@IPDToBedId)AND      
 (@IPDFromRoomId is NULL OR a.FromRoomId=@IPDFromRoomId)AND      
 (@IPDToRoomId is NULL OR a.ToRoomId=@IPDToRoomId)AND      
 (@Reason IS NULL OR (a.Reason LIKE '%' + @Reason + '%' OR @Reason LIKE '%' + a.Reason + '%')) Order by a.CreatedAt Desc   
   
END 