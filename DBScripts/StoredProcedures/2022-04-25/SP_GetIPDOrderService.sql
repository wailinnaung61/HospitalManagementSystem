select * from DepartmentType

select * from IPDOrder
select * from IPDOrderService
select * from [Service]


select * from IPDRecord
select * from AspNetUsers

insert into IPDOrder (CreatedAt,CreatedBy,IsDelete,[Date],VoucherNo,IPDRecordId,CFFee,Tax,Discount,Total,Balance,IsPaid,PaidDate)
values(GETDATE(),'9bbd3cf4-16a5-4be0-aaad-3bf6d0c46ba2',0,GETDATE(),'V-0002',3,15000,0,0,15000,0,1,GETDATE())

insert into IPDOrderService (IPDOrderId,ServiceId,FeeType,Fee,ReferralFee,UnitPrice,Qty,IsFOC,SortOrder,[Date],IPDRecordId)
values(3,1,1,15000,5000,1000,2,0,2,GETDATE(),3)