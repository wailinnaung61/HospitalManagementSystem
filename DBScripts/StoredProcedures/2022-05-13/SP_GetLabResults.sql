SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:      Aung Naing OO
-- Create Date: May 13, 2022
-- Description: Get All LabResults
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetLabResults]
    @BranchId INT = NULL,
    @LabResultId INT = NULL,
    @PatientId INT = NULL,
    @TechnicianId INT = NULL,
    @ConsultantId INT = NULL,
    @ResultNo NVARCHAR(MAX) = NULL,
    @IsCompleted BIT = NULL,
    @Date DATETIME2(7) = NULL,
    @StartDate DATETIME2(7) = NULL,
    @EndDate DATETIME2(7) = NULL,
    @CompletedDate DATETIME2(7) = NULL,
    @StartCompletedDate DATETIME2(7) = NULL,
    @EndCompletedDate DATETIME2(7) = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT O.*, P.Name AS [PatientName], P.Guardian AS [PatientGuardian], B.Name as BranchName,B.Address AS [BranchAddress],B.Phone AS [BranchPhone], T.Name AS [TechnicianName], C.Name AS [ConsultantName]  FROM [LabResult] O
        LEFT JOIN Patient P ON O.PatientId=P.Id
		LEFT JOIN LabPerson T ON O.TechnicianId=T.Id
		LEFT JOIN LabPerson C ON O.ConsultantId=C.Id
		LEFT JOIN Branch B ON O.BranchId=B.Id
        WHERE 
            (@BranchId IS NULL OR O.BranchId=@BranchId) AND
            (@LabResultId IS NULL OR O.Id=@LabResultId) AND
            (@PatientId IS NULL OR PatientId=@PatientId) AND
            (@TechnicianId IS NULL OR TechnicianId=@TechnicianId) AND
            (@ConsultantId IS NULL OR ConsultantId=@ConsultantId) AND
            (@ResultNo IS NULL OR (ResultNo LIKE '%' + @ResultNo + '%' OR @ResultNo LIKE '%' + ResultNo + '%')) AND
            (@IsCompleted IS NULL OR [IsCompleted]=@IsCompleted) AND
            (@Date IS NULL OR [Date]=@Date) AND
            (@StartDate IS NULL OR [Date]>=@StartDate) AND 
            (@EndDate IS NULL OR [Date]<=@EndDate) AND 
            (@CompletedDate IS NULL OR [CompletedDate]=@Date) AND
            (@StartCompletedDate IS NULL OR [CompletedDate]>=@StartDate) AND 
            (@EndCompletedDate IS NULL OR [CompletedDate]<=@EndDate) AND 
            O.IsDelete = 0
        ORDER BY [ResultNo] DESC
END
GO
