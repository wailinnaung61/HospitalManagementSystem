SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: May 13, 2022
-- Description: Get All LabOrders
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetLabOrders]
    @BranchId INT = NULL,
    @LabOrderId INT = NULL,
    @PatientId INT = NULL,
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

    SELECT O.*, P.Name AS [PatientName], P.Guardian AS [PatientGuardian], B.Name as BranchName,B.Address AS [BranchAddress],B.Phone AS [BranchPhone]  FROM [LabOrder] O
        LEFT JOIN Patient P ON O.PatientId=P.Id
		LEFT JOIN Branch B ON O.BranchId=B.Id
        WHERE 
            (@BranchId IS NULL OR O.BranchId=@BranchId) AND
            (@LabOrderId IS NULL OR O.Id=@LabOrderId) AND
            (@PatientId IS NULL OR PatientId=@PatientId) AND
            (@VoucherNo IS NULL OR (VoucherNo LIKE '%' + @VoucherNo + '%' OR @VoucherNo LIKE '%' + VoucherNo + '%')) AND
            (@Date IS NULL OR [Date]=@Date) AND
            (@StartDate IS NULL OR [Date]>=@StartDate) AND 
            (@EndDate IS NULL OR [Date]<=@EndDate) AND 
            (@PaidDate IS NULL OR [PaidDate]=@Date) AND
            (@StartPaidDate IS NULL OR [PaidDate]>=@StartDate) AND 
            (@EndPaidDate IS NULL OR [PaidDate]<=@EndDate) AND 
            O.IsDelete = 0
        ORDER BY [VoucherNo] DESC
END
GO
