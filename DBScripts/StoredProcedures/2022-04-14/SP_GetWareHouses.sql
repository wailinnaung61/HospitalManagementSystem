  
Alter PROCEDURE [dbo].[SP_GetWareHouses]  
    @BranchId INT = NULL,  
    @WareHouseId INT = NULL  
  
AS  
BEGIN  
  
    SET NOCOUNT ON  
  
    SELECT * FROM WareHouse   
        WHERE   
             (@BranchId is null or BranchId=@BranchId) AND  
			 (@WareHouseId is null or Id=@WareHouseId) AND  
            IsDelete = 0  
END