SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[SP_GetMedicalRecords]
    @BranchId INT = NULL,
    @MedicalRecordId INT = NULL,
    @VisitId INT = NULL,
    @PatientId INT = NULL,
    @DoctorId INT = NULL,
    @Date DATETIME2(7) = NULL,
    @StartDate DATETIME2(7) = NULL,
    @EndDate DATETIME2(7) = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT MR.*, P.Name AS [PatientName], D.Name AS [DoctorName]
    FROM MedicalRecord MR
        LEFT JOIN Visit V ON MR.VisitId=V.Id
        LEFT JOIN Patient P ON MR.PatientId=P.Id
        LEFT JOIN Doctor D ON MR.DoctorId=D.Id
    WHERE 
        (@BranchId IS NULL OR MR.BranchId=@BranchId)AND
        (@MedicalRecordId IS NULL OR MR.Id=@MedicalRecordId)AND
        (@VisitId IS NULL OR MR.VisitId=@VisitId)AND
        (@PatientId IS NULL OR MR.PatientId=@PatientId)AND
        (@DoctorId IS NULL OR MR.DoctorId=@DoctorId)AND
        (@Date IS NULL OR MR.[Date]=@Date)AND
        (@StartDate IS NULL OR MR.[Date]>=@StartDate)AND
        (@EndDate IS NULL OR MR.[Date]<=@EndDate)AND
        MR.IsDelete = 0
    ORDER BY [Date], [CreatedAt] DESC
END
GO
