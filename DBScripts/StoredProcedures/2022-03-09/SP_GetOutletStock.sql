-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zun Pwint Phyu
-- Create date: Mar 10 2022
-- Description: Get stock from outlet
-- =============================================
CREATE PROCEDURE SP_GetOutletStock
	-- Add the parameters for the stored procedure here
	@BranchId int=null,
	@WarehouseId int=null,
	@OutletId int=null,
	@ItemId int=null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   select OT.*,I.Name as ItemName,I.Code as ItemCode,O.Name as OutletName,W.Id as WarehouseId, W.Name as WarehouseName,B.Name as BranchName,B.Id as BranchId from OutletItem OT join Outlet O on O.Id=OT.OutletId
	join Item I on I.Id=OT.ItemId
	join Warehouse W on W.Id=O.WarehouseId 
	join Branch B on B.Id=O.BranchId
	
	where 
	(@BranchId is null Or O.BranchId=@BranchId) And
	(@WarehouseId is null Or O.WarehouseId=@WarehouseId) And
	(@OutletId is null Or O.Id=@OutletId) And
	(@ItemId is null Or OT.ItemId=@ItemId) 


END
GO
