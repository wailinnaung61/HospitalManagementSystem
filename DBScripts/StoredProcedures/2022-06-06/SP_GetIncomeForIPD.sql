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
-- Create date: 2022,June 06
-- Description:	<Description,,>
-- =============================================
Alter PROCEDURE SP_GetIncomeForIPD
	-- Add the parameters for the stored procedure here
	@BranchId int=null,
	@StartDate DATETIME2(7) = NULL,
    @EndDate DATETIME2(7) = NULL
AS
BEGIN
		Create Table #tmpTable(OrderIncome decimal(18,2),LabOrderIncome decimal(18,2),IPDIncome decimal(18,2),OTIncome decimal(18,2)) 
	Declare @ipdAmt decimal(18,2)
	SET NOCOUNT ON;

    -- Insert statements for procedure here
if((select count(*) from #tmpTable)>0)
begin
update #tmpTable set IPDIncome=(select Sum(IPDP.Amount) from IPDRecord IR left join IPDPayment IPDP on IR.Id=IPDP.IPDRecordId where (@BranchId IS NULL OR IR.BranchId=@BranchId) AND 
 (@StartDate IS NULL OR Convert(date,IR.PaidDate)>=Convert(date,@StartDate)) AND 
            (@EndDate IS NULL OR Convert(date,IR.PaidDate)<=Convert(date,@EndDate)) AND 
            IR.IsDelete = 0 and IR.IsPaid=1)
end
else
begin
insert into #tmpTable (IPDIncome)
(select Sum(IPDP.Amount) from IPDRecord IR left join IPDPayment IPDP on IR.Id=IPDP.IPDRecordId where (@BranchId IS NULL OR IR.BranchId=@BranchId) AND 
 (@StartDate IS NULL OR Convert(date,IR.PaidDate)>=Convert(date,@StartDate)) AND 
            (@EndDate IS NULL OR Convert(date,IR.PaidDate)<=Convert(date,@EndDate)) AND 
            IR.IsDelete = 0 and IR.IsPaid=1)
end
select * from #tmpTable
drop table #tmpTable
END
GO
