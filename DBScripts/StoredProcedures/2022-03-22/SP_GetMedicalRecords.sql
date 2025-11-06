SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_GetMedicalRecords]
    @BranchId INT = NULL,
    @MedicalRecordId INT = NULL,
    @VisitId INT = NULL,
    @PatientId INT = NULL,
    @DoctorId INT = NULL
AS
BEGIN

    SET NOCOUNT ON

    SELECT MR.*
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
        MR.IsDelete = 0
    ORDER BY [Date], [CreatedAt] DESC
END
GO
