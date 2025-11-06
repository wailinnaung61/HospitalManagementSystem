/****** Object:  StoredProcedure [dbo].[SP_GetSaleItem]    Script Date: 6/1/2022 9:00:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Zun Pwint Phyu
-- Create date: 2022,May 31
-- Description:	<Description,,>
-- =============================================
Alter PROCEDURE [dbo].[SP_GetOrderService] 
	-- Add the parameters for the stored procedure here
	@BranchId int=null,
	@FromDate datetime=null,
	@ToDate datetime=null,
	@OutletId int=null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select S.Id,S.Name As ServiceName,S.Code ServiceCode,Sum(OS.Qty) as Qty,Sum(OS.Qty*OS.UnitPrice) as Amount,Ol.Name as OutletName,sum(R.Fee) as ReferrerFee from [Order] O 
join OrderService OS on OS.OrderId=O.Id
join Outlet Ol on Ol.Id=O.OutletId
join [Service] S on S.Id=OS.ServiceId
left join Referrer R on R.Id=OS.ReferrerId
where
(@BranchId IS NULL Or O.BranchId=@BranchId) AND
(@FromDate IS NULL Or O.Date>=@FromDate) AND
(@ToDate Is NULL Or O.Date<=@ToDate) AND
(@OutletId IS NULL Or O.OutletId=@OutletId) AND
O.IsDelete=0
group by S.Id,S.Name,S.Code,Ol.Name
END
