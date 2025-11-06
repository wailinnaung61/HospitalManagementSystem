Alter PROCEDURE [dbo].[SP_GetIPDRecords]  
    @IPDRecordId INT = NULL,
    @Doa DATETIME2(7) = NULL,  
    @Dodc DATETIME2(7) = NULL,
	@PatientId INT = NULL,  
	@IsPaid INT = NULL,
	@PaidDate DATETIME2(7) = NULL,
	@PaymentType INT = NULL,
	@Status NVARCHAR(MAX) = NULL,
	@RoomId INT = NULL,
	@BedId INT = NULL,
	@Remark NVARCHAR(MAX) = NULL,  
	@Discount INT = NULL,
	@Tax INT = NULL,
	@DepartmentId INT = NULL,
	@VoucherNo NVARCHAR(MAX) = NULL,
	@TreatmentProcess int=null
AS  
BEGIN  
  
    SET NOCOUNT ON  
  
	Select i.*,p.Image from IPDRecord i left join Patient p on i.PatientId=p.Id 
        WHERE   
            (@IPDRecordId IS NULL OR i.Id=@IPDRecordId) AND 			
            (@Doa IS NULL OR i.DOA=@Doa) AND  
            (@Dodc IS NULL OR i.DODC=@Dodc) AND  			
			(@PatientId IS NULL OR i.PatientId=@PatientId) AND  
			(@IsPaid IS NULL OR i.IsPaid=@IsPaid) AND
			(@PaidDate IS NULL OR i.[PaidDate]=@PaidDate) AND  
			(@PaymentType IS NULL OR PaymentType=@PaymentType) AND			
			(@Status IS NULL OR i.[Status]=@Status) AND						
			(@RoomId IS NULL OR i.RoomId=@RoomId) AND
			(@BedId IS NULL OR i.BedId=@BedId) AND
			(@Remark IS NULL OR i.Remark=@Remark) AND						
			(@Discount IS NULL OR i.Discount=@Discount) AND			
			(@Tax IS NULL OR i.Tax=@Tax) AND
			(@DepartmentId IS NULL OR i.DepartmentId=@DepartmentId) AND
			(@VoucherNo IS NULL OR (i.VoucherNo LIKE '%' + @VoucherNo + '%' OR @VoucherNo LIKE '%' + i.VoucherNo + '%')) AND       
			(@TreatmentProcess IS NULL OR  i.IPDStatusEnum=@TreatmentProcess) And
            i.IsDelete = 0  
			ORDER BY [VoucherNo] DESC  
END