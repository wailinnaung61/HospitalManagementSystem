SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: Feb 25, 2022
-- Description: Get All Orders
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetOrders]
    @BranchId INT = NULL,
    @OutletId INT = NULL,
    @OrderId INT = NULL,
    @PatientId INT = NULL,
    @DoctorId INT = NULL,
    @VoucherNo NVARCHAR(MAX) = NULL,
    @IsPaid BIT = NULL,
    @Date DATETIME2(7) = NULL,
    @StartDate DATETIME2(7) = NULL,
    @EndDate DATETIME2(7) = NULL,
    @PaidDate DATETIME2(7) = NULL,
    @StartPaidDate DATETIME2(7) = NULL,
    @EndPaidDate DATETIME2(7) = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT * FROM [Order] 
        WHERE 
            (@BranchId IS NULL OR BranchId=@BranchId) AND
            (@OutletId IS NULL OR OutletId=@OutletId) AND
            (@OrderId IS NULL OR Id=@OrderId) AND
            (@PatientId IS NULL OR PatientId=@PatientId) AND
            (@DoctorId IS NULL OR DoctorId=@DoctorId) AND
            (@VoucherNo IS NULL OR (VoucherNo LIKE '%' + @VoucherNo + '%' OR @VoucherNo LIKE '%' + VoucherNo + '%')) AND
            (@Date IS NULL OR [Date]=@Date) AND
            (@StartDate IS NULL OR [Date]>=@StartDate) AND 
            (@EndDate IS NULL OR [Date]<=@EndDate) AND 
            (@PaidDate IS NULL OR [PaidDate]=@Date) AND
            (@StartPaidDate IS NULL OR [PaidDate]>=@StartDate) AND 
            (@EndPaidDate IS NULL OR [PaidDate]<=@EndDate) AND 
            IsDelete = 0
        ORDER BY [VoucherNo] DESC
END
GO
