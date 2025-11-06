CREATE PROCEDURE [dbo].[SP_GetAvailableRoomsandBeds]      
@DepartmentId INT = NULL           
AS      
BEGIN          
SET NOCOUNT ON  
  
select r.*,w.Name as WardName,rt.Name as RoomTypeName from Room r   
join Ward w on w.Id=r.WardId   
join Floor f on f.Id=w.FloorId  
join Department d on w.DepartmentId=d.Id  
join Branch b on d.BranchId=b.Id  
join RoomType rt on rt.Id=r.RoomTypeId  
where r.Status='Available' and d.Id=@DepartmentId and r.IsDelete=0  
  
End